namespace Kysect.Shreks.Application.Abstractions.Exceptions;

public class SubmissionAlreadyExistsException : ShreksApplicationException
{
    public SubmissionAlreadyExistsException(string? message) : base(message) { }
}