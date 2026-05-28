using System.Collections.Concurrent;
using BlastPlanning.Application.Abstractions.EventStore;
using BlastPlanning.Domain.Events;

namespace BlastPlanning.Infrastructure.EventStore.InMemory;

public sealed class InMemoryEventStore : IEventStore
{
    private readonly ConcurrentDictionary<string, List<IDomainEvent>> _streams = new();

    public Task<IReadOnlyCollection<IDomainEvent>> LoadStreamAsync(
        string streamId,
        CancellationToken cancellationToken = default)
    {
        var events = _streams.TryGetValue(streamId, out var stream)
            ? stream.ToList()
            : [];

        return Task.FromResult<IReadOnlyCollection<IDomainEvent>>(events);
    }

    public Task AppendToStreamAsync(
        string streamId,
        long expectedVersion,
        IReadOnlyCollection<IDomainEvent> events,
        CancellationToken cancellationToken = default)
    {
        var stream = _streams.GetOrAdd(streamId, _ => []);

        lock (stream)
        {
            var currentVersion = stream.Count - 1;

            if (currentVersion != expectedVersion)
            {
                throw new InvalidOperationException(
                    $"Concurrency conflict for stream '{streamId}'. Expected version {expectedVersion}, actual version {currentVersion}.");
            }

            stream.AddRange(events);
        }

        return Task.CompletedTask;
    }
}