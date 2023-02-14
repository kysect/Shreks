namespace ITMO.Dev.ASAP.Common.Exceptions;

public class UnsupportedOperationException : DomainException
{
    public UnsupportedOperationException(string? message)
        : base(message) { }
}