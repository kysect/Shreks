namespace Kysect.Shreks.Core.Exceptions;

public class DomainInvalidOperationException : ShreksDomainException
{
    public DomainInvalidOperationException(string? message) : base(message) { }
}