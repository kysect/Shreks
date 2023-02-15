using ITMO.Dev.ASAP.Core.Study;
using ITMO.Dev.ASAP.Core.SubjectCourseAssociations;
using ITMO.Dev.ASAP.Core.UserAssociations;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ITMO.Dev.ASAP.Tests.Handlers;

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