namespace BlastPlanning.Infrastructure.EventStore.Cosmos;

public sealed class CosmosEventStoreOptions
{
    public const string SectionName = "CosmosEventStore";

    public string ConnectionString { get; init; } = string.Empty;
    public string DatabaseName { get; init; } = string.Empty;
    public string ContainerName { get; init; } = string.Empty;
}