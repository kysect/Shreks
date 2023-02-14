using FluentAssertions;
using ITMO.Dev.ASAP.Application.Tools;
using ITMO.Dev.ASAP.Core.Queue;
using ITMO.Dev.ASAP.Core.Queue.Building;
using ITMO.Dev.ASAP.Core.Study;
using ITMO.Dev.ASAP.Core.Submissions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ITMO.Dev.ASAP.Tests.Queue;

public class QueueFilterTest : TestBase
{
    [Fact]
    public async Task DefaultQueue_Should_NotThrow()
    {
        SubjectCourse subjectCourse = await Context.SubjectCourses.FirstAsync();

        StudentGroup group = subjectCourse.Groups
            .Select(x => x.StudentGroup)
            .First(group => subjectCourse.Assignments
                .SelectMany(x => x.GroupAssignments)
                .SelectMany(x => x.Submissions)
                .Any(x => x.Student.Group?.Equals(group) ?? false));

        SubmissionQueue queue = new DefaultQueueBuilder(group, subjectCourse.Id).Build();

        IEnumerable<Submission> submissions = await queue.UpdateSubmissions(
            Context.Submissions, new QueryExecutor(), default);

        submissions.Should().NotBeEmpty();
    }
}