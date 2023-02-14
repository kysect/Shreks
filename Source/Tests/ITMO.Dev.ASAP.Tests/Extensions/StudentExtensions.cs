using ITMO.Dev.ASAP.Core.UserAssociations;
using ITMO.Dev.ASAP.Core.Users;

namespace ITMO.Dev.ASAP.Tests.Extensions;

public static class StudentExtensions
{
    public static string GetRepositoryName(this Student student)
    {
        return student.User.Associations
            .OfType<GithubUserAssociation>()
            .First()
            .GithubUsername;
    }
}