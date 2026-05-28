using BlastPlanning.Application.Abstractions.Clock;

namespace BlastPlanning.Infrastructure.Clock;

public sealed class SystemClock : IClock
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}