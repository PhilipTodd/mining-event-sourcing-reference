using BlastPlanning.Application.Abstractions;
using BlastPlanning.Application.Abstractions.ReadModels;

namespace BlastPlanning.Application.BlastPlans.Queries.GetRecentBlastPlans;

public sealed class GetRecentBlastPlansQueryHandler(
    IBlastPlanReadRepository repository)
{
    private const int MaximumLimit = 20;

    public Task<IReadOnlyList<RecentBlastPlanSummary>> HandleAsync(
        GetRecentBlastPlansQuery query,
        CancellationToken cancellationToken = default)
    {
        var limit = Math.Clamp(query.Limit, 1, MaximumLimit);

        return repository.GetRecentAsync(limit, cancellationToken);
    }
}