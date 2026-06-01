using BlastPlanning.Application.BlastPlans.Queries.GetBlastPlanSummary;

namespace BlastPlanning.Application.Abstractions.ReadModels;

public interface IBlastPlanReadRepository
{
    Task<BlastPlanSummaryDto?> GetAsync(
        Guid blastPlanId,
        CancellationToken cancellationToken = default);

    Task SaveAsync(
        BlastPlanSummaryDto summary,
        CancellationToken cancellationToken = default);
}