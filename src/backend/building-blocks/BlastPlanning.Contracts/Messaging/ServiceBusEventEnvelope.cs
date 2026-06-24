namespace BlastPlanning.Contracts.Messaging;

public sealed record ServiceBusEventEnvelope(
    string StreamId,
    string EventType,
    int EventVersion,
    DateTimeOffset OccurredUtc,
    string Data);