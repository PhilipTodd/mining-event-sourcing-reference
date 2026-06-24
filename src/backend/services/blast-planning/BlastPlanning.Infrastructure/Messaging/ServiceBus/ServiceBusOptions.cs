namespace BlastPlanning.Infrastructure.Messaging.ServiceBus;

public sealed class ServiceBusOptions
{
    public const string SectionName = "ServiceBus";

    public string ConnectionString { get; init; } = string.Empty;
    public string TopicName { get; init; } = "domain-events";
    public string SubscriptionName { get; init; } = "blast-plan-projections";
}