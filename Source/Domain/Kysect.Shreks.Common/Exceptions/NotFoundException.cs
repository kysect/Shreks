namespace Kysect.Shreks.Common.Exceptions;

public abstract class NotFoundException : ShreksDomainException
{
    protected NotFoundException() { }

    protected NotFoundException(string? message)
        : base(message) { }

    protected NotFoundException(string? message, Exception? innerException)
        : base(message, innerException) { }
}