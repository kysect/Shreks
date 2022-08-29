namespace Kysect.Shreks.Common.Exceptions;

public class UserWasNotFoundByGithubUsernameException : ShreksDomainException
{
    public UserWasNotFoundByGithubUsernameException(string githubUsername)
        : base($"Cannot find user with github {githubUsername}. All users should be registered before submitting tasks.")
    {
    }
}