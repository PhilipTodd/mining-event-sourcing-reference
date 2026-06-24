using Azure.Messaging.ServiceBus;
using BlastPlanning.Application.Abstractions.Messaging;
using BlastPlanning.Domain.Events;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace BlastPlanning.Infrastructure.Messaging.ServiceBus;

public sealed class ServiceBusEventPublisher(
    ServiceBusClient serviceBusClient,
    IOptions<ServiceBusOptions> options) : IEventPublisher
{
    private static readonly JsonSerializerOptions JsonOptions =
        new(JsonSerializerDefaults.Web);

    public async Task PublishAsync(
        string streamId,
        IReadOnlyCollection<IDomainEvent> events,
        CancellationToken cancellationToken = default)
    {
        await using var sender = serviceBusClient.CreateSender(options.Value.TopicName);

        foreach (var domainEvent in events)
        {
            var envelope = new ServiceBusEventEnvelope(
                StreamId: streamId,
                EventType: domainEvent.GetType().Name,
                EventVersion: 1,
                OccurredUtc: domainEvent.OccurredUtc,
                Data: JsonSerializer.Serialize(
                    domainEvent,
                    domainEvent.GetType(),
                    JsonOptions));

            var body = JsonSerializer.Serialize(envelope, JsonOptions);

            var message = new ServiceBusMessage(body)
            {
                ContentType = "application/json",
                Subject = envelope.EventType,
                MessageId = $"{streamId}-{envelope.EventType}-{envelope.OccurredUtc:O}"
            };

            message.ApplicationProperties["streamId"] = streamId;
            message.ApplicationProperties["eventType"] = envelope.EventType;
            message.ApplicationProperties["eventVersion"] = envelope.EventVersion;

            await sender.SendMessageAsync(message, cancellationToken);
        }
    }
}

public sealed record ServiceBusEventEnvelope(
    string StreamId,
    string EventType,
    int EventVersion,
    DateTimeOffset OccurredUtc,
    string Data);