namespace BlastPlanning.ProjectionWorker.Messaging;

public sealed record ServiceBusEventEnvelope(
    string StreamId,
    string EventType,
    int EventVersion,
    DateTimeOffset OccurredUtc,
    string Data);