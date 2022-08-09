namespace Kysect.Shreks.Application.Common.Exceptions;

public class ShreksApplicationException : Exception
{
    public ShreksApplicationException() { }
    public ShreksApplicationException(string? message) : base(message) { }
    public ShreksApplicationException(string? message, Exception? innerException) : base(message, innerException) { }
}