using Kysect.Shreks.Core.UserAssociations;

namespace Kysect.Shreks.Core.SubjectCourseAssociations;

public static class GithubSubjectCourseAssociationExtensions
{
    public static IReadOnlyCollection<GithubUserAssociation> GetAllGithubUsers(this GithubSubjectCourseAssociation association)
    {
        return association
            .SubjectCourse
            .Groups
            .Select(g => g.StudentGroup)
            .SelectMany(g => g.Students)
            .Select(u => u.User)
            .SelectMany(u => u.Associations)
            .OfType<GithubUserAssociation>()
            .ToList();
    }
}