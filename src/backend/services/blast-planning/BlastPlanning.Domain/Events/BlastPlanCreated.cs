using BlastPlanning.Domain.ValueObjects;

namespace BlastPlanning.Domain.Events;

public sealed record BlastPlanCreated(
    BlastPlanId BlastPlanId,
    string Name,
    string SiteId,
    DateTimeOffset OccurredUtc
) : IDomainEvent;