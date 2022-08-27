namespace Kysect.Shreks.Application.Abstractions.Exceptions;

public class InvalidUserInputException : ShreksApplicationException
{
    public InvalidUserInputException(string? message) : base(message) { }
}