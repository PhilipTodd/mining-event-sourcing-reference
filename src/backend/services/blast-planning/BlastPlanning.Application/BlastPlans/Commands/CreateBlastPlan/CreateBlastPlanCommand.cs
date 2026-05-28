using MediatR;

namespace BlastPlanning.Application.BlastPlans.Commands.CreateBlastPlan;

public sealed record CreateBlastPlanCommand(
    string Name,
    string SiteId) : IRequest<CreateBlastPlanResult>;