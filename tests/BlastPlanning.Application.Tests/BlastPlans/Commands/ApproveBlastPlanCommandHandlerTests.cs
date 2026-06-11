using BlastPlanning.Application.Abstractions.Clock;
using BlastPlanning.Application.Abstractions.EventStore;
using BlastPlanning.Application.BlastPlans.Commands.ApproveBlastPlan;
using BlastPlanning.Application.Common.Exceptions;
using BlastPlanning.Domain.Events;
using BlastPlanning.Domain.ValueObjects;
using FluentAssertions;

namespace BlastPlanning.Application.Tests.BlastPlans.Commands;

public sealed class ApproveBlastPlanCommandHandlerTests
{
    [Fact]
    public async Task Handle_Should_Append_BlastPlanApproved_Event()
    {
        var blastPlanId = Guid.NewGuid();
        var eventStore = new FakeEventStore();
        var clock = new FakeClock(new DateTimeOffset(2026, 5, 26, 10, 0, 0, TimeSpan.Zero));

        await eventStore.AppendToStreamAsync(
            $"blast-plan-{blastPlanId}",
            -1,
            [
                new BlastPlanCreated(
                    new BlastPlanId(blastPlanId),
                    "Test Blast Plan",
                    "site-001",
                    clock.UtcNow.AddMinutes(-10))
            ]);

        var handler = new ApproveBlastPlanCommandHandler(eventStore, clock);

        await handler.Handle(
            new ApproveBlastPlanCommand(blastPlanId, "user-001"),
            CancellationToken.None);

        var events = await eventStore.LoadStreamAsync($"blast-plan-{blastPlanId}");

        events.Should().HaveCount(2);

        var approved = events.Last().Should().BeOfType<BlastPlanApproved>().Subject;

        approved.BlastPlanId.Value.Should().Be(blastPlanId);
        approved.ApprovedBy.Should().Be("user-001");
        approved.ApprovedUtc.Should().Be(clock.UtcNow);
        approved.OccurredUtc.Should().Be(clock.UtcNow);
    }

    [Fact]
    public async Task Handle_When_Stream_Does_Not_Exist_Should_Throw()
    {
        var eventStore = new FakeEventStore();
        var clock = new FakeClock(DateTimeOffset.UtcNow);

        var handler = new ApproveBlastPlanCommandHandler(eventStore, clock);

        var act = async () => await handler.Handle(
            new ApproveBlastPlanCommand(Guid.NewGuid(), "user-001"),
            CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    private sealed class FakeClock(DateTimeOffset utcNow) : IClock
    {
        public DateTimeOffset UtcNow { get; } = utcNow;
    }

    private sealed class FakeEventStore : IEventStore
    {
        private readonly Dictionary<string, List<IDomainEvent>> _streams = new();

        public Task<IReadOnlyCollection<IDomainEvent>> LoadStreamAsync(
            string streamId,
            CancellationToken cancellationToken = default)
        {
            var events = _streams.TryGetValue(streamId, out var stream)
                ? stream.ToList()
                : [];

            return Task.FromResult<IReadOnlyCollection<IDomainEvent>>(events);
        }

        public Task AppendToStreamAsync(
            string streamId,
            long expectedVersion,
            IReadOnlyCollection<IDomainEvent> events,
            CancellationToken cancellationToken = default)
        {
            var stream = _streams.TryGetValue(streamId, out var existing)
                ? existing
                : _streams[streamId] = [];

            var currentVersion = stream.Count - 1;

            if (currentVersion != expectedVersion)
            {
                throw new ConcurrencyException("Concurrency conflict.");
            }

            stream.AddRange(events);

            return Task.CompletedTask;
        }
    }
}