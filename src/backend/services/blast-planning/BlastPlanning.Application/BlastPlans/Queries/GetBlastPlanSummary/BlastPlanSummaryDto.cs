namespace BlastPlanning.Application.BlastPlans.Queries.GetBlastPlanSummary;

public sealed record BlastPlanSummaryDto(
    Guid BlastPlanId,
    string Name,
    string SiteId,
    string Status,
    DateTimeOffset CreatedUtc,
    DateTimeOffset? ApprovedUtc);