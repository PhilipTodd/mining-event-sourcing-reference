using BlastPlanning.Application.Abstractions.ReadModels;
using BlastPlanning.Application.BlastPlans.Queries.GetBlastPlanSummary;
using BlastPlanning.Application.BlastPlans.Queries.GetRecentBlastPlans;
using BlastPlanning.Infrastructure.Persistence.Sql;
using Dapper;
//using System.Data;

namespace BlastPlanning.Infrastructure.Projections.Sql;

public sealed class SqlBlastPlanReadRepository(
    SqlConnectionFactory connectionFactory)
    : IBlastPlanReadRepository
{
    public async Task<BlastPlanSummaryDto?> GetAsync(
        Guid blastPlanId,
        CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT
                BlastPlanId,
                Name,
                SiteId,
                Status,
                CreatedUtc,
                ApprovedUtc
            FROM dbo.BlastPlanSummary
            WHERE BlastPlanId = @BlastPlanId;
            """;

        await using var connection = connectionFactory.CreateConnection();

        return await connection.QuerySingleOrDefaultAsync<BlastPlanSummaryDto>(
            new CommandDefinition(
                sql,
                new { BlastPlanId = blastPlanId },
                cancellationToken: cancellationToken));
    }

    public async Task<IReadOnlyList<RecentBlastPlanSummary>> GetRecentAsync(
    int limit,
    CancellationToken cancellationToken = default)
    {
        const string sql = """
        SELECT TOP (@Limit)
            BlastPlanId,
            Name,
            SiteId,
            Status,
            CreatedUtc,
            ApprovedUtc
        FROM dbo.BlastPlanSummary
        ORDER BY CreatedUtc DESC;
        """;

        //using var connection = await connectionFactory.CreateConnectionAsync(cancellationToken);
        await using var connection = connectionFactory.CreateConnection();

        var command = new CommandDefinition(
            sql,
            new { Limit = limit },
            cancellationToken: cancellationToken);

        var results = await connection.QueryAsync<RecentBlastPlanSummary>(command);

        return results.AsList();
    }

    public async Task SaveAsync(
        BlastPlanSummaryDto summary,
        CancellationToken cancellationToken = default)
    {
        const string sql = """
            MERGE dbo.BlastPlanSummary AS target
            USING
            (
                SELECT
                    @BlastPlanId AS BlastPlanId,
                    @Name AS Name,
                    @SiteId AS SiteId,
                    @Status AS Status,
                    @CreatedUtc AS CreatedUtc,
                    @ApprovedUtc AS ApprovedUtc
            ) AS source
            ON target.BlastPlanId = source.BlastPlanId
            WHEN MATCHED THEN
                UPDATE SET
                    Name = source.Name,
                    SiteId = source.SiteId,
                    Status = source.Status,
                    CreatedUtc = source.CreatedUtc,
                    ApprovedUtc = source.ApprovedUtc
            WHEN NOT MATCHED THEN
                INSERT
                (
                    BlastPlanId,
                    Name,
                    SiteId,
                    Status,
                    CreatedUtc,
                    ApprovedUtc
                )
                VALUES
                (
                    source.BlastPlanId,
                    source.Name,
                    source.SiteId,
                    source.Status,
                    source.CreatedUtc,
                    source.ApprovedUtc
                );
            """;

        await using var connection = connectionFactory.CreateConnection();

        await connection.ExecuteAsync(
            new CommandDefinition(
                sql,
                summary,
                cancellationToken: cancellationToken));
    }
}