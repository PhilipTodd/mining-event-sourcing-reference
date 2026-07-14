using BlastPlanning.Application.Abstractions;
using BlastPlanning.Application.Abstractions.ReadModels;
using BlastPlanning.Application.BlastPlans.Queries.GetBlastPlanSummary;
using BlastPlanning.Application.BlastPlans.Queries.GetRecentBlastPlans;

namespace BlastPlanning.Api.Tests.Support;

public sealed class FakeBlastPlanReadRepository
    : IBlastPlanReadRepository
{
    private readonly List<RecentBlastPlanSummary> _items = [];

    public void SetItems(
        IEnumerable<RecentBlastPlanSummary> items)
    {
        _items.Clear();
        _items.AddRange(items);
    }

    public Task<IReadOnlyList<RecentBlastPlanSummary>> GetRecentAsync(
        int limit,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        IReadOnlyList<RecentBlastPlanSummary> result = _items
            .OrderByDescending(item => item.CreatedUtc)
            .Take(limit)
            .ToArray();

        return Task.FromResult(result);
    }

    public Task<BlastPlanSummaryDto?> GetAsync(
        Guid blastPlanId,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("GetAsync() is not used by this test.");
    }

    public Task SaveAsync(
        BlastPlanSummaryDto summary,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("SaveAsync() is not used by this test.");
    }
}