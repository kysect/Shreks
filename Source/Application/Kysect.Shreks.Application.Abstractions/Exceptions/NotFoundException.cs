namespace Kysect.Shreks.Application.Abstractions.Exceptions;

public abstract class NotFoundException : ShreksApplicationException
{
    protected NotFoundException() : base() { }
    protected NotFoundException(string? message) : base(message) { }
    protected NotFoundException(string? message, Exception? innerException) : base(message, innerException) { }
}