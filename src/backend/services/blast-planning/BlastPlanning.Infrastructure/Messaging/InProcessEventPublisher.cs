using BlastPlanning.Application.Abstractions.Messaging;
using BlastPlanning.Domain.Events;
using BlastPlanning.Infrastructure.Projections.BlastPlans;

namespace BlastPlanning.Infrastructure.Messaging;

public sealed class InProcessEventPublisher(
    BlastPlanProjector projector) : IEventPublisher
{
    public async Task PublishAsync(
        string streamId,
        IReadOnlyCollection<IDomainEvent> events,
        CancellationToken cancellationToken = default)
    {
        foreach (var domainEvent in events)
        {
            await projector.ProjectAsync(
                domainEvent,
                cancellationToken);
        }
    }
}