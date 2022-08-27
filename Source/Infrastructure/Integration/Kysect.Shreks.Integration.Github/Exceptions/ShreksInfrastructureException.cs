namespace Kysect.Shreks.Integration.Github.Exceptions;

public abstract class ShreksInfrastructureException : Exception
{
    protected ShreksInfrastructureException() { }
    protected ShreksInfrastructureException(string? message) : base(message) { }
    protected ShreksInfrastructureException(string? message, Exception? inner) : base(message, inner) { }
}