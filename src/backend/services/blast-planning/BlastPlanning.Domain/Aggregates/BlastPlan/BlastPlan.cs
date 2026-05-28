using BlastPlanning.Domain.Common;
using BlastPlanning.Domain.Events;
using BlastPlanning.Domain.Exceptions;
using BlastPlanning.Domain.ValueObjects;

namespace BlastPlanning.Domain.Aggregates.BlastPlan;

public sealed class BlastPlan : AggregateRoot
{
    public BlastPlanId Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string SiteId { get; private set; } = string.Empty;
    public BlastPlanStatus Status { get; private set; }

    private BlastPlan()
    {
    }

    public static BlastPlan Create(
        BlastPlanId id,
        string name,
        string siteId,
        DateTimeOffset occurredUtc)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainValidationException("Blast plan name is required.");
        }

        var blastPlan = new BlastPlan();

        blastPlan.Raise(new BlastPlanCreated(
            id,
            name,
            siteId,
            occurredUtc));

        return blastPlan;
    }

    public void Approve(string approvedBy, DateTimeOffset approvedUtc)
    {
        if (Status != BlastPlanStatus.Draft)
        {
            throw new InvalidBlastPlanStateException(
                $"Only draft blast plans can be approved. Current status is '{Status}'.");
        }

        Raise(new BlastPlanApproved(
            Id,
            approvedBy,
            approvedUtc,
            approvedUtc));
    }

    protected override void Apply(IDomainEvent domainEvent)
    {
        switch (domainEvent)
        {
            case BlastPlanCreated e:
                Id = e.BlastPlanId;
                Name = e.Name;
                SiteId = e.SiteId;
                Status = BlastPlanStatus.Draft;
                break;

            case BlastPlanApproved:
                Status = BlastPlanStatus.Approved;
                break;

            default:
                throw new InvalidOperationException(
                    $"Unsupported domain event '{domainEvent.GetType().Name}'.");
        }
    }

    public static BlastPlan FromHistory(IEnumerable<IDomainEvent> events)
    {
        var blastPlan = new BlastPlan();
        blastPlan.LoadFromHistory(events);
        return blastPlan;
    }
}