using System.Net;
using System.Net.Http.Json;
using BlastPlanning.Api.Tests.Support;
using BlastPlanning.Application.BlastPlans.Queries.GetRecentBlastPlans;

namespace BlastPlanning.Api.Tests.Endpoints;

public sealed class GetRecentBlastPlansEndpointTests
    : IClassFixture<BlastPlanningApiFactory>
{
    private readonly BlastPlanningApiFactory _factory;
    private readonly HttpClient _client;

    public GetRecentBlastPlansEndpointTests(
        BlastPlanningApiFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetRecentBlastPlans_ReturnsSuccessWithoutAuthentication()
    {
        // Arrange
        _factory.ReadRepository.SetItems([]);

        using var request = new HttpRequestMessage(
            HttpMethod.Get,
            "/blast-plans/recent");

        // Deliberately do not provide an Authorization header.

        // Act
        using var response = await _client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetRecentBlastPlans_ReturnsExpectedFields()
    {
        // Arrange
        var blastPlanId = Guid.NewGuid();

        var createdUtc = new DateTime(
            2026,
            7,
            14,
            1,
            15,
            42,
            DateTimeKind.Utc);

        var approvedUtc = new DateTime(
            2026,
            7,
            14,
            2,
            40,
            10,
            DateTimeKind.Utc);

        _factory.ReadRepository.SetItems(
        [
            new RecentBlastPlanSummary(
                blastPlanId,
                "North Pit Bench 14",
                "SITE-001",
                "Approved",
                createdUtc,
                approvedUtc)
        ]);

        // Act
        var result = await _client
            .GetFromJsonAsync<List<RecentBlastPlanSummary>>(
                "/blast-plans/recent");

        // Assert
        Assert.NotNull(result);

        var blastPlan = Assert.Single(result);

        Assert.Equal(blastPlanId, blastPlan.BlastPlanId);
        Assert.Equal("North Pit Bench 14", blastPlan.Name);
        Assert.Equal("SITE-001", blastPlan.SiteId);
        Assert.Equal("Approved", blastPlan.Status);
        Assert.Equal(createdUtc, blastPlan.CreatedUtc);
        Assert.Equal(approvedUtc, blastPlan.ApprovedUtc);
    }

    [Fact]
    public async Task GetRecentBlastPlans_ReturnsMaximumTwentyItems()
    {
        // Arrange
        var items = Enumerable
            .Range(1, 25)
            .Select(index =>
                new RecentBlastPlanSummary(
                    Guid.NewGuid(),
                    $"Blast Plan {index}",
                    "SITE-001",
                    "Draft",
                    new DateTime(
                        2026,
                        7,
                        1,
                        0,
                        0,
                        0,
                        DateTimeKind.Utc)
                        .AddMinutes(index),
                    ApprovedUtc: null))
            .ToArray();

        _factory.ReadRepository.SetItems(items);

        // Act
        var result = await _client
            .GetFromJsonAsync<List<RecentBlastPlanSummary>>(
                "/blast-plans/recent");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(20, result.Count);
    }

    [Fact]
    public async Task GetRecentBlastPlans_ReturnsNewestItemsFirst()
    {
        // Arrange
        var oldest = CreateBlastPlan(
            name: "Oldest",
            createdUtc: new DateTime(
                2026,
                7,
                1,
                1,
                0,
                0,
                DateTimeKind.Utc));

        var newest = CreateBlastPlan(
            name: "Newest",
            createdUtc: new DateTime(
                2026,
                7,
                3,
                1,
                0,
                0,
                DateTimeKind.Utc));

        var middle = CreateBlastPlan(
            name: "Middle",
            createdUtc: new DateTime(
                2026,
                7,
                2,
                1,
                0,
                0,
                DateTimeKind.Utc));

        _factory.ReadRepository.SetItems(
        [
            oldest,
            newest,
            middle
        ]);

        // Act
        var result = await _client
            .GetFromJsonAsync<List<RecentBlastPlanSummary>>(
                "/blast-plans/recent");

        // Assert
        Assert.NotNull(result);

        Assert.Collection(
            result,
            item => Assert.Equal("Newest", item.Name),
            item => Assert.Equal("Middle", item.Name),
            item => Assert.Equal("Oldest", item.Name));
    }

    [Fact]
    public async Task GetRecentBlastPlans_ReturnsNullApprovedUtcForDraft()
    {
        // Arrange
        _factory.ReadRepository.SetItems(
        [
            new RecentBlastPlanSummary(
                Guid.NewGuid(),
                "Draft Blast Plan",
                "SITE-001",
                "Draft",
                DateTime.UtcNow,
                ApprovedUtc: null)
        ]);

        // Act
        var result = await _client
            .GetFromJsonAsync<List<RecentBlastPlanSummary>>(
                "/blast-plans/recent");

        // Assert
        Assert.NotNull(result);

        var blastPlan = Assert.Single(result);

        Assert.Null(blastPlan.ApprovedUtc);
    }

    [Fact]
    public async Task GetRecentBlastPlans_ReturnsEmptyArrayWhenNoItemsExist()
    {
        // Arrange
        _factory.ReadRepository.SetItems([]);

        // Act
        var result = await _client
            .GetFromJsonAsync<List<RecentBlastPlanSummary>>(
                "/blast-plans/recent");

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    private static RecentBlastPlanSummary CreateBlastPlan(
        string name,
        DateTime createdUtc)
    {
        return new RecentBlastPlanSummary(
            Guid.NewGuid(),
            name,
            "SITE-001",
            "Draft",
            createdUtc,
            ApprovedUtc: null);
    }
}