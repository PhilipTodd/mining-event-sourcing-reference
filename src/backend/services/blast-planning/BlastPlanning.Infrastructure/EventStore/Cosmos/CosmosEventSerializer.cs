using System.Text.Json;
using BlastPlanning.Domain.Events;

namespace BlastPlanning.Infrastructure.EventStore.Cosmos;

public sealed class CosmosEventSerializer
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public CosmosEventDocument Serialize(
        string streamId,
        string aggregateId,
        string aggregateType,
        long sequence,
        IDomainEvent domainEvent)
    {
        return new CosmosEventDocument(
            id: $"{streamId}-{sequence:D10}",
            streamId: streamId,
            aggregateId: aggregateId,
            aggregateType: aggregateType,
            sequence: sequence,
            eventType: domainEvent.GetType().Name,
            eventVersion: 1,
            occurredUtc: domainEvent.OccurredUtc,
            data: JsonSerializer.Serialize(domainEvent, domainEvent.GetType(), JsonOptions));
    }

    public IDomainEvent Deserialize(CosmosEventDocument document)
    {
        return document.eventType switch
        {
            nameof(BlastPlanCreated) =>
                JsonSerializer.Deserialize<BlastPlanCreated>(document.data, JsonOptions)!,

            nameof(BlastPlanApproved) =>
                JsonSerializer.Deserialize<BlastPlanApproved>(document.data, JsonOptions)!,

            _ => throw new InvalidOperationException(
                $"Unsupported event type '{document.eventType}'.")
        };
    }
}