using Azure.Messaging.ServiceBus;
using BlastPlanning.Infrastructure.Projections.BlastPlans;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace BlastPlanning.ProjectionFunction.Functions;

public sealed class ProcessBlastPlanEvent(
    ProjectionProcessor projectionProcessor,
    ILogger<ProcessBlastPlanEvent> logger)
{
    [Function(nameof(ProcessBlastPlanEvent))]
    public async Task RunAsync(
        [ServiceBusTrigger(
            "%ServiceBusTopicName%",
            "%ServiceBusSubscriptionName%",
            Connection = "ServiceBusConnection")]
        ServiceBusReceivedMessage message,
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Received Service Bus message {MessageId}. " +
            "Subject: {Subject}, Delivery count: {DeliveryCount}, " +
            "Enqueued time: {EnqueuedTimeUtc}",
            message.MessageId,
            message.Subject,
            message.DeliveryCount,
            message.EnqueuedTime);

        try
        {
            await projectionProcessor.ProcessAsync(
                message,
                cancellationToken);

            logger.LogInformation(
                "Successfully processed Service Bus message {MessageId}.",
                message.MessageId);
        }
        catch (Exception exception)
        {
            logger.LogError(
                exception,
                "Failed to process Service Bus message {MessageId}. " +
                "Delivery count: {DeliveryCount}",
                message.MessageId,
                message.DeliveryCount);

            throw;
        }
    }
}