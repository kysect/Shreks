using FluentAssertions;
using Kysect.Shreks.Application.Tools;
using Kysect.Shreks.Core.Queue;
using Kysect.Shreks.Core.Queue.Building;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Core.Submissions;
using Kysect.Shreks.Tests.Application;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Kysect.Shreks.Tests.Queue;

public class QueueFilterTest : ApplicationTestBase
{
    [Fact]
    public async Task DefaultQueue_Should_NotThrow()
    {
        StudentGroup group = await Context.StudentGroups.FirstAsync();
        SubjectCourse subjectCourse = await Context.SubjectCourses.FirstAsync();

        SubmissionQueue queue = new DefaultQueueBuilder(group, subjectCourse.Id).Build();

        IEnumerable<Submission> submissions = await queue.UpdateSubmissions(
            Context.Submissions, new QueryExecutor(), default);

        submissions.Should().NotBeNull();
    }
}