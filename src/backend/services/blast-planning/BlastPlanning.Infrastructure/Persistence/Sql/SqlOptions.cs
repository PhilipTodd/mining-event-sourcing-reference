namespace BlastPlanning.Infrastructure.Persistence.Sql;

public sealed class SqlOptions
{
    public const string SectionName = "Sql";

    public string ConnectionString { get; init; } = string.Empty;
}