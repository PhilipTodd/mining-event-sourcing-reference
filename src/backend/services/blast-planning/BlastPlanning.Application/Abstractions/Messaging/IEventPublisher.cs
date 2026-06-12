using BlastPlanning.Domain.Events;

namespace BlastPlanning.Application.Abstractions.Messaging;

public interface IEventPublisher
{
    Task PublishAsync(
        string streamId,
        IReadOnlyCollection<IDomainEvent> events,
        CancellationToken cancellationToken = default);
}