namespace BlastPlanning.Application.Common.Exceptions;

public sealed class NotFoundException(string message) : ApplicationException(message);