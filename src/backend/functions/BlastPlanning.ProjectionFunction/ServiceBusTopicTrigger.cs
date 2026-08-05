using System;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace BlastPlanning.ProjectionFunction;

public class ServiceBusTopicTrigger
{
    private readonly ILogger<ServiceBusTopicTrigger> _logger;

    public ServiceBusTopicTrigger(ILogger<ServiceBusTopicTrigger> logger)
    {
        _logger = logger;
    }

    [Function(nameof(ServiceBusTopicTrigger))]
    public async Task Run(
        [ServiceBusTrigger("mytopic", "mysubscription", Connection = "")]
        ServiceBusReceivedMessage message,
        ServiceBusMessageActions messageActions)
    {
        _logger.LogInformation("Message ID: {id}", message.MessageId);
        _logger.LogInformation("Message Body: {body}", message.Body);
        _logger.LogInformation("Message Content-Type: {contentType}", message.ContentType);

            // Complete the message
        await messageActions.CompleteMessageAsync(message);
    }
}