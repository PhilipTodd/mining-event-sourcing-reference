namespace BlastPlanning.Infrastructure.EventStore.Cosmos;

public sealed record CosmosEventDocument(
    string id,
    string streamId,
    string aggregateId,
    string aggregateType,
    long sequence,
    string eventType,
    int eventVersion,
    DateTimeOffset occurredUtc,
    string data);