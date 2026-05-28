using BlastPlanning.Application.Abstractions.Clock;
using BlastPlanning.Application.Abstractions.EventStore;
using BlastPlanning.Domain.Aggregates.BlastPlan;
using BlastPlanning.Domain.ValueObjects;
using MediatR;

namespace BlastPlanning.Application.BlastPlans.Commands.CreateBlastPlan;

public sealed class CreateBlastPlanCommandHandler(
    IEventStore eventStore,
    IClock clock)
    : IRequestHandler<CreateBlastPlanCommand, CreateBlastPlanResult>
{
    public async Task<CreateBlastPlanResult> Handle(
        CreateBlastPlanCommand request,
        CancellationToken cancellationToken)
    {
        var blastPlanId = BlastPlanId.New();

        var blastPlan = BlastPlan.Create(
            blastPlanId,
            request.Name,
            request.SiteId,
            clock.UtcNow);

        await eventStore.AppendToStreamAsync(
            streamId: $"blast-plan-{blastPlanId.Value}",
            expectedVersion: -1,
            events: blastPlan.UncommittedEvents,
            cancellationToken);

        blastPlan.ClearUncommittedEvents();

        return new CreateBlastPlanResult(blastPlanId.Value);
    }
}