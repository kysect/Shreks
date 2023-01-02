namespace Kysect.Shreks.Common.Exceptions;

public class SubmissionAlreadyExistsException : ShreksDomainException
{
    public SubmissionAlreadyExistsException(long prNumber)
        : base($"Submission for PR-{prNumber} already exists") { }
}