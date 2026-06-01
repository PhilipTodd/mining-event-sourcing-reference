using BlastPlanning.Application.Abstractions.ReadModels;
using BlastPlanning.Application.BlastPlans.Queries.GetBlastPlanSummary;
using FluentAssertions;

namespace BlastPlanning.Application.Tests.BlastPlans.Queries;

public sealed class GetBlastPlanSummaryQueryHandlerTests
{
    [Fact]
    public async Task Handle_When_ReadModel_Exists_Should_Return_Summary()
    {
        var blastPlanId = Guid.NewGuid();

        var expected = new BlastPlanSummaryDto(
            blastPlanId,
            "Test Blast Plan",
            "site-001",
            "Approved",
            DateTimeOffset.UtcNow.AddMinutes(-10),
            DateTimeOffset.UtcNow);

        var repository = new FakeBlastPlanReadRepository(expected);

        var handler = new GetBlastPlanSummaryQueryHandler(repository);

        var result = await handler.Handle(
            new GetBlastPlanSummaryQuery(blastPlanId),
            CancellationToken.None);

        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task Handle_When_ReadModel_Does_Not_Exist_Should_Return_Null()
    {
        var repository = new FakeBlastPlanReadRepository(null);

        var handler = new GetBlastPlanSummaryQueryHandler(repository);

        var result = await handler.Handle(
            new GetBlastPlanSummaryQuery(Guid.NewGuid()),
            CancellationToken.None);

        result.Should().BeNull();
    }

    private sealed class FakeBlastPlanReadRepository(
        BlastPlanSummaryDto? summary) : IBlastPlanReadRepository
    {
        public Task<BlastPlanSummaryDto?> GetAsync(
            Guid blastPlanId,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(summary);
        }

        public Task SaveAsync(
            BlastPlanSummaryDto summary,
            CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }
    }
}