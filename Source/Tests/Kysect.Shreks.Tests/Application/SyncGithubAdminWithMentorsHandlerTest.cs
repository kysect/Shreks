using FluentAssertions;
using Kysect.Shreks.Application.Abstractions.Submissions.Commands;
using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Application.Handlers.Submissions;
using Kysect.Shreks.Core.Models;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Core.SubjectCourseAssociations;
using Kysect.Shreks.Core.UserAssociations;
using Kysect.Shreks.Integration.Google;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using Xunit;

namespace Kysect.Shreks.Tests.Application;

public class SyncGithubAdminWithMentorsHandlerTest : ApplicationTestBase
{
    [Fact]
    public async Task Handle_Should_UpdateSubmissionState()
    {
        SubjectCourse subjectCourse = Context.SubjectCourses.First(sa => sa.Associations.OfType<GithubSubjectCourseAssociation>().Any());

        var currentMentors = await Context
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