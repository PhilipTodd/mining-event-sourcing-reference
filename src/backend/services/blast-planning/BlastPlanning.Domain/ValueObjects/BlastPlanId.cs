namespace BlastPlanning.Domain.ValueObjects;

public readonly record struct BlastPlanId(Guid Value)
{
    public static BlastPlanId New() => new(Guid.NewGuid());

    public override string ToString() => Value.ToString();
}