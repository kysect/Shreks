namespace Kysect.Shreks.Core.Exceptions;

public class UserWasNotFoundByGithubUsernameException : ShreksDomainException
{
    public UserWasNotFoundByGithubUsernameException(string githubUsername)
        : base($"Cannot find user with github {githubUsername}. All users should be registered before submitting tasks.")
    {
    }
}