using BlastPlanning.Application.Abstractions.Clock;
using BlastPlanning.Application.Abstractions.EventStore;
using BlastPlanning.Application.BlastPlans.Commands.CreateBlastPlan;
using BlastPlanning.Application.Common.Exceptions;
using BlastPlanning.Domain.Events;
using FluentAssertions;

namespace BlastPlanning.Application.Tests.BlastPlans.Commands;

public sealed class CreateBlastPlanCommandHandlerTests
{
    [Fact]
    public async Task Handle_Should_Append_BlastPlanCreated_Event()
    {
        var eventStore = new FakeEventStore();
        var clock = new FakeClock(new DateTimeOffset(2026, 5, 26, 10, 0, 0, TimeSpan.Zero));

        var handler = new CreateBlastPlanCommandHandler(eventStore, clock);

        var result = await handler.Handle(
            new CreateBlastPlanCommand("Test Blast Plan", "site-001"),
            CancellationToken.None);

        result.BlastPlanId.Should().NotBeEmpty();

        var streamId = $"blast-plan-{result.BlastPlanId}";
        var events = await eventStore.LoadStreamAsync(streamId);

        events.Should().ContainSingle();

        var created = events.Single().Should().BeOfType<BlastPlanCreated>().Subject;

        created.BlastPlanId.Value.Should().Be(result.BlastPlanId);
        created.Name.Should().Be("Test Blast Plan");
        created.SiteId.Should().Be("site-001");
        created.OccurredUtc.Should().Be(clock.UtcNow);
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