using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Core.SubjectCourseAssociations;
using Kysect.Shreks.Core.UserAssociations;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Kysect.Shreks.Tests.Handlers;

public class SyncGithubAdminWithMentorsHandlerTest : TestBase
{
    [Fact]
    public async Task Handle_Should_UpdateSubmissionState()
    {
        SubjectCourse subjectCourse =
            Context.SubjectCourses.First(sa => sa.Associations.OfType<GithubSubjectCourseAssociation>().Any());

        List<string> currentMentors = await Context
            .SubjectCourses
            .Where(s => s.Id == subjectCourse.Id)
            .SelectMany(a => a.Mentors)
            .Select(m => m.User)
            .SelectMany(m => m.Associations)
            .OfType<GithubUserAssociation>()
            .Select(a => a.GithubUsername)
            .ToListAsync();
    }
}