namespace ITMO.Dev.ASAP.Common.Exceptions;

public abstract class NotFoundException : DomainException
{
    protected NotFoundException() { }

    protected NotFoundException(string? message)
        : base(message) { }

    protected NotFoundException(string? message, Exception? innerException)
        : base(message, innerException) { }
}