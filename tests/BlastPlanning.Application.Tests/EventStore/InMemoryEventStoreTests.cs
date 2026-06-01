using BlastPlanning.Application.Abstractions.ReadModels;
using BlastPlanning.Application.BlastPlans.Queries.GetBlastPlanSummary;
using BlastPlanning.Domain.Events;
using BlastPlanning.Domain.ValueObjects;
using BlastPlanning.Infrastructure.EventStore.InMemory;
using BlastPlanning.Infrastructure.Projections.BlastPlans;
using FluentAssertions;

namespace BlastPlanning.Application.Tests.EventStore;

public sealed class InMemoryEventStoreTests
{
    [Fact]
    public async Task AppendToStreamAsync_Should_Append_Events()
    {
        var eventStore = CreateEventStore();

        var blastPlanId = BlastPlanId.New();
        var streamId = $"blast-plan-{blastPlanId.Value}";

        var created = new BlastPlanCreated(
            blastPlanId,
            "Test Blast Plan",
            "site-001",
            DateTimeOffset.UtcNow);

        await eventStore.AppendToStreamAsync(
            streamId,
            -1,
            [created],
            CancellationToken.None);

        var events = await eventStore.LoadStreamAsync(streamId);

        events.Should().ContainSingle();
        events.Single().Should().BeEquivalentTo(created);
    }

    [Fact]
    public async Task AppendToStreamAsync_When_ExpectedVersion_Does_Not_Match_Should_Throw()
    {
        var eventStore = CreateEventStore();

        var blastPlanId = BlastPlanId.New();
        var streamId = $"blast-plan-{blastPlanId.Value}";

        var created = new BlastPlanCreated(
            blastPlanId,
            "Test Blast Plan",
            "site-001",
            DateTimeOffset.UtcNow);

        var act = async () => await eventStore.AppendToStreamAsync(
            streamId,
            5,
            [created],
            CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*Concurrency conflict*");
    }

    [Fact]
    public async Task AppendToStreamAsync_Should_Project_Events()
    {
        var repository = new FakeBlastPlanReadRepository();
        var projector = new BlastPlanProjector(repository);
        var eventStore = new InMemoryEventStore(projector);

        var blastPlanId = BlastPlanId.New();
        var streamId = $"blast-plan-{blastPlanId.Value}";

        var created = new BlastPlanCreated(
            blastPlanId,
            "Test Blast Plan",
            "site-001",
            DateTimeOffset.UtcNow);

        await eventStore.AppendToStreamAsync(
            streamId,
            -1,
            [created],
            CancellationToken.None);

        var summary = await repository.GetAsync(blastPlanId.Value);

        summary.Should().NotBeNull();
        summary!.Name.Should().Be("Test Blast Plan");
        summary.Status.Should().Be("Draft");
    }

    private static InMemoryEventStore CreateEventStore()
    {
        var repository = new FakeBlastPlanReadRepository();
        var projector = new BlastPlanProjector(repository);

        return new InMemoryEventStore(projector);
    }

    private sealed class FakeBlastPlanReadRepository : IBlastPlanReadRepository
    {
        private readonly Dictionary<Guid, BlastPlanSummaryDto> _summaries = new();

        public Task<BlastPlanSummaryDto?> GetAsync(
            Guid blastPlanId,
            CancellationToken cancellationToken = default)
        {
            _summaries.TryGetValue(blastPlanId, out var summary);
            return Task.FromResult(summary);
        }

        public Task SaveAsync(
            BlastPlanSummaryDto summary,
            CancellationToken cancellationToken = default)
        {
            _summaries[summary.BlastPlanId] = summary;
            return Task.CompletedTask;
        }
    }
}