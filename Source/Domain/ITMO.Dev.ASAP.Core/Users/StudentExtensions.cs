using ITMO.Dev.ASAP.Core.UserAssociations;

namespace ITMO.Dev.ASAP.Core.Users;

public static class StudentExtensions
{
    public static GithubUserAssociation AddGithubAssociation(this Student student, string githubUsername)
    {
        return new GithubUserAssociation(Guid.NewGuid(), student.User, githubUsername);
    }
}