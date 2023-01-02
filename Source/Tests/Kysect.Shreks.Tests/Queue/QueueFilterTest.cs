using FluentAssertions;
using Kysect.Shreks.Application.Tools;
using Kysect.Shreks.Core.Queue;
using Kysect.Shreks.Core.Queue.Building;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Core.Submissions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Kysect.Shreks.Tests.Queue;

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