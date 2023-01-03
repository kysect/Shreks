using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Core.UserAssociations;
using Microsoft.EntityFrameworkCore;

namespace Kysect.Shreks.Application.DatabaseContextExtensions;

public static class GithubSubjectCourseAssociationExtensions
{
    public static async Task<IReadOnlyCollection<GithubUserAssociation>> GetAllGithubUsers(
        this DbSet<SubjectCourse> subjectCourses,
        Guid subjectCourseId)
    {
        return await subjectCourses
            .Where(sc => sc.Id == subjectCourseId)
            .SelectMany(sc => sc.Groups)
            .Select(g => g.StudentGroup)
            .SelectMany(g => g.Students)
            .Select(u => u.User)
            .SelectMany(u => u.Associations)
            .OfType<GithubUserAssociation>()
            .ToListAsync();
    }
}