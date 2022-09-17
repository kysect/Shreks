using Kysect.Shreks.Core.UserAssociations;

namespace Kysect.Shreks.Core.Users;

public static class StudentExtensions
{
    public static GithubUserAssociation AddGithubAssociation(this Student student, string githubUsername)
    {
        return new GithubUserAssociation(student.User, githubUsername);
    }
}