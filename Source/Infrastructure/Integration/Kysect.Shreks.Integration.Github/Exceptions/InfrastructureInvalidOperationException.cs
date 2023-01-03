namespace Kysect.Shreks.Integration.Github.Exceptions;

public class InfrastructureInvalidOperationException : ShreksInfrastructureException
{
    public InfrastructureInvalidOperationException(string? message) : base(message) { }
}