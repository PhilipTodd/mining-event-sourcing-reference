using BlastPlanning.Application.Abstractions.Clock;
using BlastPlanning.Application.Abstractions.EventStore;
using BlastPlanning.Domain.Aggregates.BlastPlan;
using MediatR;

namespace BlastPlanning.Application.BlastPlans.Commands.ApproveBlastPlan;

public sealed class ApproveBlastPlanCommandHandler(
    IEventStore eventStore,
    IClock clock)
    : IRequestHandler<ApproveBlastPlanCommand>
{
    public async Task Handle(
        ApproveBlastPlanCommand request,
        CancellationToken cancellationToken)
    {
        var streamId = $"blast-plan-{request.BlastPlanId}";

        var events = await eventStore.LoadStreamAsync(
            streamId,
            cancellationToken);

        if (events.Count == 0)
        {
            throw new InvalidOperationException(
                $"Blast plan '{request.BlastPlanId}' was not found.");
        }
        
        var blastPlan = BlastPlan.FromHistory(events);

        blastPlan.Approve(
            request.ApprovedBy,
            clock.UtcNow);

        await eventStore.AppendToStreamAsync(
            streamId,
            blastPlan.Version,
            blastPlan.UncommittedEvents,
            cancellationToken);

        blastPlan.ClearUncommittedEvents();
    }
}