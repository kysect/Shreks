using Kysect.Shreks.Core.UserAssociations;
using Kysect.Shreks.Core.Users;

namespace Kysect.Shreks.Tests.Extensions;

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