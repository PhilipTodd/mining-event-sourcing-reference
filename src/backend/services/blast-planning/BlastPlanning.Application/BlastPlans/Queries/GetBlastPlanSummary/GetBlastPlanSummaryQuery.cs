using MediatR;

namespace BlastPlanning.Application.BlastPlans.Queries.GetBlastPlanSummary;

public sealed record GetBlastPlanSummaryQuery(Guid BlastPlanId)
    : IRequest<BlastPlanSummaryDto?>;