using BlastPlanning.Application.Abstractions;
using BlastPlanning.Application.Abstractions.ReadModels;
using BlastPlanning.Application.BlastPlans.Queries.GetRecentBlastPlans;
using Moq;

namespace BlastPlanning.Application.Tests.BlastPlans.Queries;

public sealed class GetRecentBlastPlansQueryHandlerTests
{
    private readonly Mock<IBlastPlanReadRepository> _repository = new();

    [Fact]
    public async Task HandleAsync_ReturnsItemsFromRepository()
    {
        // Arrange
        var expected = new List<RecentBlastPlanSummary>
        {
            new(
                BlastPlanId: Guid.NewGuid(),
                Name: "North Pit Bench 14",
                SiteId: "SITE-001",
                Status: "Approved",
                CreatedUtc: new DateTime(
                    2026,
                    7,
                    14,
                    1,
                    15,
                    42,
                    DateTimeKind.Utc),
                ApprovedUtc: new DateTime(
                    2026,
                    7,
                    14,
                    2,
                    40,
                    10,
                    DateTimeKind.Utc))
        };

        _repository
            .Setup(repository => repository.GetRecentAsync(
                20,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var handler = new GetRecentBlastPlansQueryHandler(
            _repository.Object);

        var query = new GetRecentBlastPlansQuery(Limit: 20);

        // Act
        var result = await handler.HandleAsync(
            query,
            CancellationToken.None);

        // Assert
        Assert.Same(expected, result);

        _repository.Verify(repository => repository.GetRecentAsync(
                20,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Theory]
    [InlineData(100, 20)]
    [InlineData(21, 20)]
    [InlineData(20, 20)]
    [InlineData(10, 10)]
    [InlineData(1, 1)]
    [InlineData(0, 1)]
    [InlineData(-5, 1)]
    public async Task HandleAsync_ClampsLimitToValidRange(
        int requestedLimit,
        int expectedLimit)
    {
        // Arrange
        _repository
            .Setup(repository => repository.GetRecentAsync(
                expectedLimit,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        var handler = new GetRecentBlastPlansQueryHandler(
            _repository.Object);

        var query = new GetRecentBlastPlansQuery(requestedLimit);

        // Act
        await handler.HandleAsync(
            query,
            CancellationToken.None);

        // Assert
        _repository.Verify(repository => repository.GetRecentAsync(
                expectedLimit,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_PassesCancellationTokenToRepository()
    {
        // Arrange
        using var cancellationTokenSource =
            new CancellationTokenSource();

        var cancellationToken =
            cancellationTokenSource.Token;

        _repository
            .Setup(repository => repository.GetRecentAsync(
                20,
                cancellationToken))
            .ReturnsAsync([]);

        var handler = new GetRecentBlastPlansQueryHandler(
            _repository.Object);

        // Act
        await handler.HandleAsync(
            new GetRecentBlastPlansQuery(),
            cancellationToken);

        // Assert
        _repository.Verify(repository => repository.GetRecentAsync(
                20,
                cancellationToken),
            Times.Once);
    }
}