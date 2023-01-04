namespace Kysect.Shreks.Common.Exceptions;

public abstract class ShreksDomainException : Exception
{
    protected ShreksDomainException() { }

    protected ShreksDomainException(string? message) : base(message) { }

    protected ShreksDomainException(string? message, Exception? innerException) : base(message, innerException) { }
}