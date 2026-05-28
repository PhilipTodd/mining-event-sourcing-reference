using BlastPlanning.Domain.ValueObjects;

namespace BlastPlanning.Domain.Events;

public sealed record BlastPlanApproved(
    BlastPlanId BlastPlanId,
    string ApprovedBy,
    DateTimeOffset ApprovedUtc,
    DateTimeOffset OccurredUtc
) : IDomainEvent;