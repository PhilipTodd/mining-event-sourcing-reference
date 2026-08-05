using Azure.Messaging.ServiceBus;
using BlastPlanning.Contracts.Messaging;
using BlastPlanning.Infrastructure.Messaging;
using BlastPlanning.Infrastructure.Projections.BlastPlans;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace BlastPlanning.Infrastructure.Projections.BlastPlans;

public sealed class ProjectionProcessor(
    BlastPlanProjector projector,
    EventDeserializer eventDeserializer,
    ILogger<ProjectionProcessor> logger)
{
    private static readonly JsonSerializerOptions JsonOptions =
        new(JsonSerializerDefaults.Web);

    public async Task ProcessAsync(
        ServiceBusReceivedMessage message,
        CancellationToken cancellationToken = default)
    {
        var body = message.Body.ToString();

        var envelope = JsonSerializer.Deserialize<ServiceBusEventEnvelope>(
            body,
            JsonOptions);

        if (envelope is null)
        {
            throw new InvalidOperationException(
                "Failed to deserialize Service Bus event envelope.");
        }

        logger.LogInformation(
            "Processing event {EventType} from stream {StreamId}",
            envelope.EventType,
            envelope.StreamId);

        var domainEvent = eventDeserializer.Deserialize(envelope);

        await projector.ProjectAsync(
            domainEvent,
            cancellationToken);

        logger.LogInformation(
            "Projected event {EventType} from stream {StreamId}",
            envelope.EventType,
            envelope.StreamId);
    }
}