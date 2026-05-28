using MediatR;

namespace BlastPlanning.Application.BlastPlans.Commands.ApproveBlastPlan;

public sealed record ApproveBlastPlanCommand(
    Guid BlastPlanId,
    string ApprovedBy) : IRequest;