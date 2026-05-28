namespace BlastPlanning.Domain.Events;

public interface IDomainEvent
{
    DateTimeOffset OccurredUtc { get; }
}