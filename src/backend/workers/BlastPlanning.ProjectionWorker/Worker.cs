using Azure.Messaging.ServiceBus;
using BlastPlanning.Infrastructure.Messaging.ServiceBus;
using BlastPlanning.ProjectionWorker.Services;
using Microsoft.Extensions.Options;

namespace BlastPlanning.ProjectionWorker;

public sealed class Worker(
    ServiceBusClient serviceBusClient,
    IOptions<ServiceBusOptions> serviceBusOptions,
    IServiceScopeFactory scopeFactory,
    ILogger<Worker> logger) : BackgroundService
{
    private ServiceBusProcessor? _processor;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var options = serviceBusOptions.Value;

        logger.LogInformation(
            "Service Bus config loaded. Topic={TopicName}, Subscription={SubscriptionName}, HasConnectionString={HasConnectionString}",
            options.TopicName,
            options.SubscriptionName,
            !string.IsNullOrWhiteSpace(options.ConnectionString));

        _processor = serviceBusClient.CreateProcessor(
            options.TopicName,
            options.SubscriptionName,
            new ServiceBusProcessorOptions
            {
                AutoCompleteMessages = false,
                MaxConcurrentCalls = 1
            });

        _processor.ProcessMessageAsync += ProcessMessageAsync;
        _processor.ProcessErrorAsync += ProcessErrorAsync;

        logger.LogInformation(
            "Starting Service Bus projection worker for topic {TopicName}, subscription {SubscriptionName}",
            options.TopicName,
            options.SubscriptionName);

        await _processor.StartProcessingAsync(stoppingToken);

        try
        {
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (OperationCanceledException)
        {
            logger.LogInformation("Projection worker cancellation requested.");
        }
    }

    private async Task ProcessMessageAsync(ProcessMessageEventArgs args)
    {
        using var scope = scopeFactory.CreateScope();

        var processor = scope.ServiceProvider
            .GetRequiredService<ProjectionProcessor>();

        try
        {
            await processor.ProcessAsync(
                args.Message,
                args.CancellationToken);

            await args.CompleteMessageAsync(
                args.Message,
                args.CancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Failed to process Service Bus message {MessageId}",
                args.Message.MessageId);

            await args.DeadLetterMessageAsync(
                args.Message,
                deadLetterReason: ex.GetType().Name,
                deadLetterErrorDescription: ex.Message,
                cancellationToken: args.CancellationToken);
        }
    }

    private Task ProcessErrorAsync(ProcessErrorEventArgs args)
    {
        logger.LogError(
            args.Exception,
            "Service Bus processing error. Entity: {EntityPath}, Source: {ErrorSource}, Namespace: {FullyQualifiedNamespace}",
            args.EntityPath,
            args.ErrorSource,
            args.FullyQualifiedNamespace);

        return Task.CompletedTask;
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_processor is not null)
        {
            logger.LogInformation("Stopping Service Bus projection worker.");

            await _processor.StopProcessingAsync(cancellationToken);
            await _processor.DisposeAsync();
        }

        await base.StopAsync(cancellationToken);
    }
}