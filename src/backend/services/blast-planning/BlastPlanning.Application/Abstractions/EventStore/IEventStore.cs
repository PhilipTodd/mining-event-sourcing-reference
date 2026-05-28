using BlastPlanning.Domain.Events;

namespace BlastPlanning.Application.Abstractions.EventStore;

public interface IEventStore
{
    Task<IReadOnlyCollection<IDomainEvent>> LoadStreamAsync(
        string streamId,
        CancellationToken cancellationToken = default);

    Task AppendToStreamAsync(
        string streamId,
        long expectedVersion,
        IReadOnlyCollection<IDomainEvent> events,
        CancellationToken cancellationToken = default);
}