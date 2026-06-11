using BlastPlanning.Application.Abstractions.EventStore;
using BlastPlanning.Application.Common.Exceptions;
using BlastPlanning.Domain.Events;
using BlastPlanning.Infrastructure.Projections.BlastPlans;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using System.ComponentModel;
using CosmosContainer = Microsoft.Azure.Cosmos.Container;

namespace BlastPlanning.Infrastructure.EventStore.Cosmos;

public sealed class CosmosEventStore : IEventStore
{
    private readonly CosmosContainer _container;
    private readonly CosmosEventSerializer _serializer = new();
    private readonly BlastPlanProjector _projector;

    public CosmosEventStore(
        CosmosClient cosmosClient,
        IOptions<CosmosEventStoreOptions> options,
        BlastPlanProjector projector)
    {
        var value = options.Value;

        _container = cosmosClient.GetContainer(
            value.DatabaseName,
            value.ContainerName);

        _projector = projector;
    }

    public async Task<IReadOnlyCollection<IDomainEvent>> LoadStreamAsync(
        string streamId,
        CancellationToken cancellationToken = default)
    {
        var query = new QueryDefinition("""
            SELECT * FROM c
            WHERE c.streamId = @streamId
            ORDER BY c.sequence
            """)
            .WithParameter("@streamId", streamId);

        var iterator = _container.GetItemQueryIterator<CosmosEventDocument>(
            query,
            requestOptions: new QueryRequestOptions
            {
                PartitionKey = new PartitionKey(streamId)
            });

        var events = new List<IDomainEvent>();

        while (iterator.HasMoreResults)
        {
            var response = await iterator.ReadNextAsync(cancellationToken);

            foreach (var document in response)
            {
                events.Add(_serializer.Deserialize(document));
            }
        }

        return events;
    }

    public async Task AppendToStreamAsync(
        string streamId,
        long expectedVersion,
        IReadOnlyCollection<IDomainEvent> events,
        CancellationToken cancellationToken = default)
    {
        if (events.Count == 0)
        {
            return;
        }

        var currentVersion = await GetCurrentVersionAsync(streamId, cancellationToken);

        if (currentVersion != expectedVersion)
        {
            throw new ConcurrencyException(
                $"Concurrency conflict for stream '{streamId}'. Expected version {expectedVersion}, actual version {currentVersion}.");
        }

        var aggregateId = streamId.Replace("blast-plan-", string.Empty);
        var sequence = expectedVersion;

        foreach (var domainEvent in events)
        {
            sequence++;

            var document = _serializer.Serialize(
                streamId,
                aggregateId,
                "BlastPlan",
                sequence,
                domainEvent);

            await _container.CreateItemAsync(
                document,
                new PartitionKey(streamId),
                cancellationToken: cancellationToken);

            ///This is a temporary synchronous projection approach. Insert into SQL server immediately.
            await _projector.ProjectAsync(
                domainEvent,
                cancellationToken);
        }
    }

    private async Task<long> GetCurrentVersionAsync(
        string streamId,
        CancellationToken cancellationToken)
    {
        var query = new QueryDefinition("""
            SELECT VALUE MAX(c.sequence)
            FROM c
            WHERE c.streamId = @streamId
            """)
            .WithParameter("@streamId", streamId);

        var iterator = _container.GetItemQueryIterator<long?>(
            query,
            requestOptions: new QueryRequestOptions
            {
                PartitionKey = new PartitionKey(streamId)
            });

        while (iterator.HasMoreResults)
        {
            var response = await iterator.ReadNextAsync(cancellationToken);
            var value = response.FirstOrDefault();

            return value ?? -1;
        }

        return -1;
    }
}