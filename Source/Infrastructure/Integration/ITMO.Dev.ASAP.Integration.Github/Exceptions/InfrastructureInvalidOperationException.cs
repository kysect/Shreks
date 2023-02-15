namespace ITMO.Dev.ASAP.Integration.Github.Exceptions;

public class InfrastructureInvalidOperationException : InfrastructureException
{
    public InfrastructureInvalidOperationException(string? message)
        : base(message) { }
}