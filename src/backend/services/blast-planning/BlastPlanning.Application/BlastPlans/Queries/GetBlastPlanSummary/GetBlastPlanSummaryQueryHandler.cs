using BlastPlanning.Application.Abstractions.ReadModels;
using MediatR;

namespace BlastPlanning.Application.BlastPlans.Queries.GetBlastPlanSummary;

public sealed class GetBlastPlanSummaryQueryHandler(
    IBlastPlanReadRepository readRepository)
    : IRequestHandler<GetBlastPlanSummaryQuery, BlastPlanSummaryDto?>
{
    public Task<BlastPlanSummaryDto?> Handle(
        GetBlastPlanSummaryQuery request,
        CancellationToken cancellationToken)
    {
        return readRepository.GetAsync(
            request.BlastPlanId,
            cancellationToken);
    }
}