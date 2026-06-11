namespace BlastPlanning.Application.Common.Exceptions;

public sealed class ConcurrencyException(string message) : ApplicationException(message);