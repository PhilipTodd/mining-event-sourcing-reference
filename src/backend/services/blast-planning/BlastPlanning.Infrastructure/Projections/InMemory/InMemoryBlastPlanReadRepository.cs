using BlastPlanning.Application.Abstractions.ReadModels;
using BlastPlanning.Application.BlastPlans.Queries.GetBlastPlanSummary;
using BlastPlanning.Application.BlastPlans.Queries.GetRecentBlastPlans;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

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

    public async Task<IReadOnlyList<RecentBlastPlanSummary>> GetRecentAsync(
        int limit,
        CancellationToken cancellationToken = default)
    {
        // Assuming standard sorting by CreatedUtc descending to get the "recent" plans
        IReadOnlyList<RecentBlastPlanSummary> recentPlans = _summaries.Values
            .OrderByDescending(dto => dto.CreatedUtc) // Replace with your actual sorting logic if different
            .Take(20)
            .Select(dto => new RecentBlastPlanSummary(
                dto.BlastPlanId, // If the Dto doesn't contain the Id, use KeyValuePair: pair.Key
                dto.Name,
                dto.SiteId,
                dto.Status,
                dto.CreatedUtc.UtcDateTime, // Converts DateTimeOffset to DateTime (Kind: Utc)
                dto.ApprovedUtc?.UtcDateTime // Handles the nullable DateTimeOffset?
            ))
            .ToList(); // ToList() satisfies IReadOnlyList<T>

        return recentPlans;
        //return new List<RecentBlastPlanSummary>();
    }


    public Task SaveAsync(
        BlastPlanSummaryDto summary,
        CancellationToken cancellationToken = default)
    {
        _summaries[summary.BlastPlanId] = summary;

        return Task.CompletedTask;
    }
}