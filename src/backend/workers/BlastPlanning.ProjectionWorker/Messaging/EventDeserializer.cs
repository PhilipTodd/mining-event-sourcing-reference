using System.Text.Json;
using BlastPlanning.Domain.Events;
using BlastPlanning.Domain.ValueObjects;
using BlastPlanning.Contracts.Messaging;

namespace BlastPlanning.ProjectionWorker.Messaging;

public sealed class EventDeserializer
{
    private static readonly JsonSerializerOptions JsonOptions =
        new(JsonSerializerDefaults.Web);

    public IDomainEvent Deserialize(ServiceBusEventEnvelope envelope)
    {
        return envelope.EventType switch
        {
            nameof(BlastPlanCreated) =>
                Deserialize<BlastPlanCreated>(envelope),

            nameof(BlastPlanApproved) =>
                Deserialize<BlastPlanApproved>(envelope),

            _ => throw new NotSupportedException(
                $"Unsupported event type '{envelope.EventType}'.")
        };
    }

    private static T Deserialize<T>(
        ServiceBusEventEnvelope envelope)
        where T : class, IDomainEvent
    {
        var domainEvent = JsonSerializer.Deserialize<T>(
            envelope.Data,
            JsonOptions);

        return domainEvent
            ?? throw new InvalidOperationException(
                $"Failed to deserialize '{typeof(T).Name}'.");
    }
}