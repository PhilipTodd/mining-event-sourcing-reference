namespace BlastPlanning.Application.BlastPlans.Queries.GetRecentBlastPlans;

public sealed record RecentBlastPlanSummary(
    Guid BlastPlanId,
    string Name,
    string SiteId,
    string Status,
    DateTimeOffset CreatedUtc,
    DateTimeOffset? ApprovedUtc);