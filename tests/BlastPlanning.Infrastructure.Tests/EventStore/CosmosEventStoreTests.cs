using BlastPlanning.Application.Abstractions.ReadModels;
using BlastPlanning.Application.BlastPlans.Queries.GetBlastPlanSummary;
using BlastPlanning.Domain.Events;
using BlastPlanning.Domain.ValueObjects;
using BlastPlanning.Infrastructure.EventStore.Cosmos;
using BlastPlanning.Infrastructure.Projections.BlastPlans;
using FluentAssertions;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace BlastPlanning.Infrastructure.Tests.EventStore;

public sealed class CosmosEventStoreTests : IAsyncLifetime
{
    private readonly string _databaseName = $"blast-planning-tests-{Guid.NewGuid():N}";

    private const string ContainerName = "events";

    private readonly CosmosClient _cosmosClient;
    private readonly CosmosEventStore _eventStore;

    public CosmosEventStoreTests()
    {
        var configuration = new ConfigurationBuilder()
            .AddUserSecrets<CosmosEventStoreTests>()
            .Build();

        var connectionString =
            configuration["Cosmos:ConnectionString"];

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Missing environment variable COSMOS_EVENTSTORE_CONNECTION_STRING.");
        }

        _cosmosClient = new CosmosClient(connectionString);

        var options = Options.Create(new CosmosEventStoreOptions
        {
            ConnectionString = connectionString,
            DatabaseName = _databaseName,
            ContainerName = ContainerName
        });

        var readRepository = new FakeBlastPlanReadRepository();

        var projector = new BlastPlanProjector(
            readRepository);

        _eventStore = new CosmosEventStore(
            _cosmosClient,
            options,
            projector);
    }

    public async Task InitializeAsync()
    {
        var database = await _cosmosClient.CreateDatabaseIfNotExistsAsync(_databaseName);

        await database.Database.CreateContainerIfNotExistsAsync(
            id: ContainerName,
            partitionKeyPath: "/streamId",
            throughput: 400);
    }

    public async Task DisposeAsync()
    {
        await _cosmosClient.GetDatabase(_databaseName).DeleteAsync();
        _cosmosClient.Dispose();
    }

    [Fact]
    public async Task AppendToStreamAsync_Should_Persist_Events_To_Cosmos()
    {
        var blastPlanId = BlastPlanId.New();
        var streamId = $"blast-plan-{blastPlanId.Value}";
        var occurredUtc = DateTimeOffset.UtcNow;

        var created = new BlastPlanCreated(
            blastPlanId,
            "Test Blast Plan",
            "site-001",
            occurredUtc);

        await _eventStore.AppendToStreamAsync(
            streamId,
            expectedVersion: -1,
            events: [created],
            CancellationToken.None);

        var events = await _eventStore.LoadStreamAsync(
            streamId,
            CancellationToken.None);

        events.Should().ContainSingle();

        var loaded = events.Single()
            .Should()
            .BeOfType<BlastPlanCreated>()
            .Subject;

        loaded.BlastPlanId.Should().Be(blastPlanId);
        loaded.Name.Should().Be("Test Blast Plan");
        loaded.SiteId.Should().Be("site-001");
        loaded.OccurredUtc.Should().Be(occurredUtc);
    }

    [Fact]
    public async Task LoadStreamAsync_Should_Return_Events_In_Sequence_Order()
    {
        var blastPlanId = BlastPlanId.New();
        var streamId = $"blast-plan-{blastPlanId.Value}";

        var createdUtc = DateTimeOffset.UtcNow.AddMinutes(-10);
        var approvedUtc = DateTimeOffset.UtcNow;

        await _eventStore.AppendToStreamAsync(
            streamId,
            expectedVersion: -1,
            events:
            [
                new BlastPlanCreated(
                    blastPlanId,
                    "Test Blast Plan",
                    "site-001",
                    createdUtc)
            ],
            CancellationToken.None);

        await _eventStore.AppendToStreamAsync(
            streamId,
            expectedVersion: 0,
            events:
            [
                new BlastPlanApproved(
                    blastPlanId,
                    "user-001",
                    approvedUtc,
                    approvedUtc)
            ],
            CancellationToken.None);

        var events = await _eventStore.LoadStreamAsync(
            streamId,
            CancellationToken.None);

        events.Should().HaveCount(2);
        events.ElementAt(0).Should().BeOfType<BlastPlanCreated>();
        events.ElementAt(1).Should().BeOfType<BlastPlanApproved>();
    }

    [Fact]
    public async Task AppendToStreamAsync_When_ExpectedVersion_Does_Not_Match_Should_Throw()
    {
        var blastPlanId = BlastPlanId.New();
        var streamId = $"blast-plan-{blastPlanId.Value}";

        var created = new BlastPlanCreated(
            blastPlanId,
            "Test Blast Plan",
            "site-001",
            DateTimeOffset.UtcNow);

        var act = async () => await _eventStore.AppendToStreamAsync(
            streamId,
            expectedVersion: 5,
            events: [created],
            CancellationToken.None);

        await act.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("*Concurrency conflict*");
    }

    private sealed class FakeBlastPlanReadRepository
    : IBlastPlanReadRepository
    {
        private readonly Dictionary<Guid, BlastPlanSummaryDto> _items = [];

        public Task<BlastPlanSummaryDto?> GetAsync(
            Guid blastPlanId,
            CancellationToken cancellationToken = default)
        {
            _items.TryGetValue(blastPlanId, out var result);

            return Task.FromResult(result);
        }

        public Task SaveAsync(
            BlastPlanSummaryDto summary,
            CancellationToken cancellationToken = default)
        {
            _items[summary.BlastPlanId] = summary;

            return Task.CompletedTask;
        }
    }

}