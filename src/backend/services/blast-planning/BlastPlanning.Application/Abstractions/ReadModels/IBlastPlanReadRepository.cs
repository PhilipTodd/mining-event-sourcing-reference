using BlastPlanning.Application.BlastPlans.Queries.GetBlastPlanSummary;
using BlastPlanning.Application.BlastPlans.Queries.GetRecentBlastPlans;

namespace BlastPlanning.Application.Abstractions.ReadModels;

public interface IBlastPlanReadRepository
{
    Task<BlastPlanSummaryDto?> GetAsync(
        Guid blastPlanId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<RecentBlastPlanSummary>> GetRecentAsync(
        int limit,
        CancellationToken cancellationToken = default);

    Task SaveAsync(
        BlastPlanSummaryDto summary,
        CancellationToken cancellationToken = default);
}