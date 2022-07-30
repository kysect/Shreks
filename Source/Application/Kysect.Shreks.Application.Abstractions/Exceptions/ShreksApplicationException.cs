namespace Kysect.Shreks.Application.Exceptions;

public class ShreksApplicationException : Exception
{
    protected ShreksApplicationException() { }
    protected ShreksApplicationException(string? message) : base(message) { }
    protected ShreksApplicationException(string? message, Exception? innerException) : base(message, innerException) { }
}