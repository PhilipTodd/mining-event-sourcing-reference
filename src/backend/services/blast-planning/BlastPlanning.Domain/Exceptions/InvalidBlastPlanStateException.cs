namespace BlastPlanning.Domain.Exceptions;

public sealed class InvalidBlastPlanStateException(string message) : DomainException(message);