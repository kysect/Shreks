namespace Kysect.Shreks.Common.Exceptions;

public class UnsupportedOperationException : ShreksDomainException
{
    public UnsupportedOperationException(string? message)
        : base(message) { }
}