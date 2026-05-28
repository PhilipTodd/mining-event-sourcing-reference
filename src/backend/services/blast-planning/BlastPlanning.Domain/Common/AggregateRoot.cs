using BlastPlanning.Domain.Events;

namespace BlastPlanning.Domain.Common;

public abstract class AggregateRoot
{
    private readonly List<IDomainEvent> _uncommittedEvents = [];

    public IReadOnlyCollection<IDomainEvent> UncommittedEvents => _uncommittedEvents.AsReadOnly();

    public long Version { get; protected set; } = -1;

    public void LoadFromHistory(IEnumerable<IDomainEvent> events)
    {
        foreach (var domainEvent in events)
        {
            Apply(domainEvent);
            Version++;
        }
    }

    protected void Raise(IDomainEvent domainEvent)
    {
        Apply(domainEvent);
        _uncommittedEvents.Add(domainEvent);
    }

    public void ClearUncommittedEvents()
    {
        _uncommittedEvents.Clear();
    }

    protected abstract void Apply(IDomainEvent domainEvent);
}