using BlastPlanning.Application.BlastPlans.Queries.GetBlastPlanSummary;
using BlastPlanning.Infrastructure.Persistence.Sql;
using BlastPlanning.Infrastructure.Projections.Sql;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace BlastPlanning.Infrastructure.Tests.Projections;

public sealed class SqlBlastPlanReadRepositoryTests : IAsyncLifetime
{
    private readonly string _connectionString;
    private readonly SqlBlastPlanReadRepository _repository;
    private readonly string _testRunId = $"test-{Guid.NewGuid():N}";

    public SqlBlastPlanReadRepositoryTests()
    {
        var configuration = new ConfigurationBuilder()
            .AddUserSecrets<SqlBlastPlanReadRepositoryTests>()
            .Build();

        _connectionString = configuration["Sql:ConnectionString"]
            ?? throw new InvalidOperationException(
                "Missing user secret 'Sql:ConnectionString'.");

        var options = Options.Create(new SqlOptions
        {
            ConnectionString = _connectionString
        });

        var connectionFactory = new SqlConnectionFactory(options);

        _repository = new SqlBlastPlanReadRepository(connectionFactory);
    }

    public async Task InitializeAsync()
    {
        await using var connection = new SqlConnection(_connectionString);

        await connection.OpenAsync();

        var command = connection.CreateCommand();

        command.CommandText = """
            IF OBJECT_ID('dbo.BlastPlanSummary', 'U') IS NULL
            BEGIN
                CREATE TABLE dbo.BlastPlanSummary
                (
                    BlastPlanId UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
                    Name NVARCHAR(200) NOT NULL,
                    SiteId NVARCHAR(100) NOT NULL,
                    Status NVARCHAR(50) NOT NULL,
                    CreatedUtc DATETIMEOFFSET NOT NULL,
                    ApprovedUtc DATETIMEOFFSET NULL
                );
            END;
            """;

        await command.ExecuteNonQueryAsync();
    }

    public async Task DisposeAsync()
    {
        await using var connection = new SqlConnection(_connectionString);

        await connection.OpenAsync();

        var command = connection.CreateCommand();

        command.CommandText = """
            DELETE FROM dbo.BlastPlanSummary
            WHERE SiteId = @SiteId;
            """;

        command.Parameters.AddWithValue("@SiteId", _testRunId);

        await command.ExecuteNonQueryAsync();
    }

    [Fact]
    public async Task SaveAsync_When_Row_Does_Not_Exist_Should_Insert_Row()
    {
        var blastPlanId = Guid.NewGuid();

        var summary = new BlastPlanSummaryDto(
            blastPlanId,
            "Test Blast Plan",
            _testRunId,
            "Draft",
            DateTimeOffset.UtcNow,
            null);

        await _repository.SaveAsync(summary);

        var result = await _repository.GetAsync(blastPlanId);

        result.Should().BeEquivalentTo(summary);
    }

    [Fact]
    public async Task SaveAsync_When_Row_Exists_Should_Update_Row()
    {
        var blastPlanId = Guid.NewGuid();
        var createdUtc = DateTimeOffset.UtcNow.AddMinutes(-10);
        var approvedUtc = DateTimeOffset.UtcNow;

        var original = new BlastPlanSummaryDto(
            blastPlanId,
            "Test Blast Plan",
            _testRunId,
            "Draft",
            createdUtc,
            null);

        var updated = original with
        {
            Status = "Approved",
            ApprovedUtc = approvedUtc
        };

        await _repository.SaveAsync(original);
        await _repository.SaveAsync(updated);

        var result = await _repository.GetAsync(blastPlanId);

        result.Should().BeEquivalentTo(updated);
    }

    [Fact]
    public async Task GetAsync_When_Row_Does_Not_Exist_Should_Return_Null()
    {
        var result = await _repository.GetAsync(Guid.NewGuid());

        result.Should().BeNull();
    }
}