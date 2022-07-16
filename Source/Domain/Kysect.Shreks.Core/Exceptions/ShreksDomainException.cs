namespace Kysect.Shreks.Core.Exceptions;

public abstract class ShreksDomainException : Exception
{
    protected ShreksDomainException() { }
    protected ShreksDomainException(string? message) : base(message) { }
    protected ShreksDomainException(string? message, Exception? innerException) : base(message, innerException) { }
}