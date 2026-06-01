using BlastPlanning.Application.Abstractions.ReadModels;
using BlastPlanning.Application.BlastPlans.Queries.GetBlastPlanSummary;
using BlastPlanning.Domain.Events;
using BlastPlanning.Domain.ValueObjects;
using BlastPlanning.Infrastructure.Projections.BlastPlans;
using FluentAssertions;

namespace BlastPlanning.Application.Tests.Projections;

public sealed class BlastPlanProjectorTests
{
    [Fact]
    public async Task ProjectAsync_When_BlastPlanCreated_Should_Create_Summary()
    {
        var repository = new FakeBlastPlanReadRepository();
        var projector = new BlastPlanProjector(repository);

        var blastPlanId = BlastPlanId.New();
        var occurredUtc = DateTimeOffset.UtcNow;

        await projector.ProjectAsync(
            new BlastPlanCreated(
                blastPlanId,
                "Test Blast Plan",
                "site-001",
                occurredUtc),
            CancellationToken.None);

        var summary = await repository.GetAsync(blastPlanId.Value);

        summary.Should().NotBeNull();
        summary!.BlastPlanId.Should().Be(blastPlanId.Value);
        summary.Name.Should().Be("Test Blast Plan");
        summary.SiteId.Should().Be("site-001");
        summary.Status.Should().Be("Draft");
        summary.CreatedUtc.Should().Be(occurredUtc);
        summary.ApprovedUtc.Should().BeNull();
    }

    [Fact]
    public async Task ProjectAsync_When_BlastPlanApproved_Should_Update_Summary()
    {
        var repository = new FakeBlastPlanReadRepository();
        var projector = new BlastPlanProjector(repository);

        var blastPlanId = BlastPlanId.New();
        var createdUtc = DateTimeOffset.UtcNow.AddMinutes(-10);
        var approvedUtc = DateTimeOffset.UtcNow;

        await repository.SaveAsync(new BlastPlanSummaryDto(
            blastPlanId.Value,
            "Test Blast Plan",
            "site-001",
            "Draft",
            createdUtc,
            null));

        await projector.ProjectAsync(
            new BlastPlanApproved(
                blastPlanId,
                "user-001",
                approvedUtc,
                approvedUtc),
            CancellationToken.None);

        var summary = await repository.GetAsync(blastPlanId.Value);

        summary.Should().NotBeNull();
        summary!.Status.Should().Be("Approved");
        summary.ApprovedUtc.Should().Be(approvedUtc);
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