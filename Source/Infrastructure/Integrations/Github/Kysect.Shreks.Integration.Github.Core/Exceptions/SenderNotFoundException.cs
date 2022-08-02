namespace Kysect.Shreks.Integration.Github.Core.Exceptions;

public class SenderNotFoundException : Exception
{
    public SenderNotFoundException() { }
    public SenderNotFoundException(string? message) : base(message) { }
    public SenderNotFoundException(string? message, Exception? innerException) : base(message, innerException) { }
}