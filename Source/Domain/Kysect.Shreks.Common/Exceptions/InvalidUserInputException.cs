namespace Kysect.Shreks.Common.Exceptions;

public class InvalidUserInputException : ShreksDomainException
{
    public InvalidUserInputException(string? message) : base(message) { }
}