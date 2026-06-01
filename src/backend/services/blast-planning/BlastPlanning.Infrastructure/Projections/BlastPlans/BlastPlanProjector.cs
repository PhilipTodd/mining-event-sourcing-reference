using BlastPlanning.Application.Abstractions.ReadModels;
using BlastPlanning.Application.BlastPlans.Queries.GetBlastPlanSummary;
using BlastPlanning.Domain.Events;

namespace BlastPlanning.Infrastructure.Projections.BlastPlans;

public sealed class BlastPlanProjector(
    IBlastPlanReadRepository readRepository)
{
    public async Task ProjectAsync(
        IDomainEvent domainEvent,
        CancellationToken cancellationToken = default)
    {
        switch (domainEvent)
        {
            case BlastPlanCreated e:
                await ProjectAsync(e, cancellationToken);
                break;

            case BlastPlanApproved e:
                await ProjectAsync(e, cancellationToken);
                break;
        }
    }

    private async Task ProjectAsync(
        BlastPlanCreated domainEvent,
        CancellationToken cancellationToken)
    {
        var summary = new BlastPlanSummaryDto(
            BlastPlanId: domainEvent.BlastPlanId.Value,
            Name: domainEvent.Name,
            SiteId: domainEvent.SiteId,
            Status: "Draft",
            CreatedUtc: domainEvent.OccurredUtc,
            ApprovedUtc: null);

        await readRepository.SaveAsync(summary, cancellationToken);
    }

    private async Task ProjectAsync(
        BlastPlanApproved domainEvent,
        CancellationToken cancellationToken)
    {
        var existing = await readRepository.GetAsync(
            domainEvent.BlastPlanId.Value,
            cancellationToken);

        if (existing is null)
        {
            return;
        }

        var updated = existing with
        {
            Status = "Approved",
            ApprovedUtc = domainEvent.ApprovedUtc
        };

        await readRepository.SaveAsync(updated, cancellationToken);
    }
}