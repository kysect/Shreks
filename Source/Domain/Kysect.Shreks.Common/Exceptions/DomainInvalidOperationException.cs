namespace Kysect.Shreks.Common.Exceptions;

public class DomainInvalidOperationException : ShreksDomainException
{
    public DomainInvalidOperationException(string? message) : base(message) { }
}