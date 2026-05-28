using BlastPlanning.Application.Abstractions.EventStore;
using BlastPlanning.Domain.Events;
using MediatR;

namespace BlastPlanning.Application.BlastPlans.Queries.GetBlastPlanSummary;

public sealed class GetBlastPlanSummaryQueryHandler(IEventStore eventStore)
    : IRequestHandler<GetBlastPlanSummaryQuery, BlastPlanSummaryDto?>
{
    public async Task<BlastPlanSummaryDto?> Handle(
        GetBlastPlanSummaryQuery request,
        CancellationToken cancellationToken)
    {
        var streamId = $"blast-plan-{request.BlastPlanId}";

        var events = await eventStore.LoadStreamAsync(
            streamId,
            cancellationToken);

        if (events.Count == 0)
        {
            return null;
        }

        Guid blastPlanId = request.BlastPlanId;
        string? name = null;
        string? siteId = null;
        string status = "Unknown";
        DateTimeOffset? createdUtc = null;
        DateTimeOffset? approvedUtc = null;

        foreach (var domainEvent in events)
        {
            switch (domainEvent)
            {
                case BlastPlanCreated e:
                    blastPlanId = e.BlastPlanId.Value;
                    name = e.Name;
                    siteId = e.SiteId;
                    status = "Draft";
                    createdUtc = e.OccurredUtc;
                    break;

                case BlastPlanApproved e:
                    status = "Approved";
                    approvedUtc = e.ApprovedUtc;
                    break;
            }
        }

        if (name is null || siteId is null || createdUtc is null)
        {
            return null;
        }

        return new BlastPlanSummaryDto(
            blastPlanId,
            name,
            siteId,
            status,
            createdUtc.Value,
            approvedUtc);
    }
}