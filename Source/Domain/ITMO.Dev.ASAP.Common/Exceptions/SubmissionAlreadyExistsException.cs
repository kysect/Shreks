namespace ITMO.Dev.ASAP.Common.Exceptions;

public class SubmissionAlreadyExistsException : DomainException
{
    public SubmissionAlreadyExistsException(long prNumber)
        : base($"Submission for PR-{prNumber} already exists") { }
}