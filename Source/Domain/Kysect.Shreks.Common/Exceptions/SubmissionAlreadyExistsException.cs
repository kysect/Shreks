namespace Kysect.Shreks.Common.Exceptions;

public class SubmissionAlreadyExistsException : ShreksDomainException
{
    public SubmissionAlreadyExistsException(string? message) : base(message) { }
}