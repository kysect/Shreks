namespace Kysect.Shreks.Common.Exceptions;

public class UserNotFoundByGithubUsernameException : ShreksDomainException
{
    public UserNotFoundByGithubUsernameException(string githubUsername)
        : base($"Cannot find user with github {githubUsername}. All users should be registered before submitting tasks.")
    {
    }
}