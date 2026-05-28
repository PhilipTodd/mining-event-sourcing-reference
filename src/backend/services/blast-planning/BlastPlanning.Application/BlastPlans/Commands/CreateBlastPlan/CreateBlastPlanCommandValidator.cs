using FluentValidation;

namespace BlastPlanning.Application.BlastPlans.Commands.CreateBlastPlan;

public sealed class CreateBlastPlanCommandValidator
    : AbstractValidator<CreateBlastPlanCommand>
{
    public CreateBlastPlanCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.SiteId)
            .NotEmpty();
    }
}