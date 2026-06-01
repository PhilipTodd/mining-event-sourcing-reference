using System.Collections.Concurrent;
using BlastPlanning.Application.Abstractions.ReadModels;
using BlastPlanning.Application.BlastPlans.Queries.GetBlastPlanSummary;

namespace BlastPlanning.Infrastructure.Projections.InMemory;

public sealed class InMemoryBlastPlanReadRepository : IBlastPlanReadRepository
{
    private readonly ConcurrentDictionary<Guid, BlastPlanSummaryDto> _summaries = new();

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