using Kysect.Shreks.Common.Resources;

namespace Kysect.Shreks.Common.Exceptions;

public class DomainInvalidOperationException : ShreksDomainException
{
    public DomainInvalidOperationException(string? message) : base(message) { }


    public static DomainInvalidOperationException UserNotFoundByGithubUsername(string githubUsername)
    {
        return new DomainInvalidOperationException(string.Format(UserMessages.UserNotFoundByGithubUsername, githubUsername));
    }
}