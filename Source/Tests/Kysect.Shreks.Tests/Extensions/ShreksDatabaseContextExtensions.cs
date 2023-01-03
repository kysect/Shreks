using Kysect.Shreks.Core.Models;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Core.SubmissionAssociations;
using Kysect.Shreks.Core.Submissions;
using Kysect.Shreks.Core.Submissions.States;
using Kysect.Shreks.Core.SubmissionStateWorkflows;
using Kysect.Shreks.Core.Users;
using Kysect.Shreks.DataAccess.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Kysect.Shreks.Tests.Extensions;

public static class ShreksDatabaseContextExtensions
{
    public static Task<Submission> GetGithubSubmissionAsync(
        this IShreksDatabaseContext context,
        params ISubmissionState[] states)
    {
        return context.SubmissionAssociations
            .OfType<GithubSubmissionAssociation>()
            .Select(x => x.Submission)
            .Where(submission => states.Any(x => x.Equals(submission.State)))
            .Where(x =>
                x.GroupAssignment.Assignment.SubjectCourse.WorkflowType ==
                SubmissionStateWorkflowType.ReviewWithDefense)
            .FirstAsync();
    }

    public static Task<User> GetNotMentorUser(
        this IShreksDatabaseContext context,
        GroupAssignment assignment,
        User excludedUser)
    {
        IEnumerable<Guid> mentors = assignment.Assignment.SubjectCourse.Mentors
            .Select(x => x.UserId);

        return context.Users
            .Where(x => x.Equals(excludedUser) == false)
            .Where(user => mentors.Contains(user.Id) == false)
            .FirstAsync();
    }

    public static Task<GroupAssignment> GetGroupAssignmentWithUnSubmittedStudents(this IShreksDatabaseContext context)
    {
        return context.GroupAssignments
            .Where(x => x.Assignment.SubjectCourse.WorkflowType == SubmissionStateWorkflowType.ReviewWithDefense)
            .Where(assignment => assignment.Group.Students.Any(student =>
                assignment.Submissions.All(submission => submission.Student.Equals(student) == false)))
            .FirstAsync();
    }

    public static Task<GroupAssignment> GetGroupAssignmentWithSubmittedStudents(this IShreksDatabaseContext context)
    {
        GroupAssignment groupAssignment = context.GroupAssignments
            .Where(x => x.Assignment.SubjectCourse.WorkflowType == SubmissionStateWorkflowType.ReviewWithDefense)
            .AsEnumerable()
            .First(assignment => assignment.Submissions.GroupBy(x => x.Student)
                .Select(x => x.OrderByDescending(xx => xx.Code).First())
                .Any(x => x.State.Kind is SubmissionStateKind.Active));

        return Task.FromResult(groupAssignment);
    }
}