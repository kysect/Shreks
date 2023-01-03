using Kysect.Shreks.Core.Models;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Core.SubjectCourseAssociations;
using Kysect.Shreks.Core.Users;

namespace Kysect.Shreks.Tests.Extensions;

public static class GroupAssignmentExtensions
{
    public static Student GetUnSubmittedStudent(this GroupAssignment assignment)
    {
        return assignment.Group.Students
            .First(student => assignment.Submissions.Any(submission => submission.Student.Equals(student)) == false);
    }

    public static Student GetSubmittedStudent(this GroupAssignment assignment)
    {
        return assignment.Submissions
            .GroupBy(x => x.Student)
            .Select(x => x.OrderByDescending(xx => xx.Code).First())
            .Where(x => x.State.Kind is SubmissionStateKind.Active)
            .Select(x => x.Student)
            .First();
    }

    public static string GetOrganizationName(this GroupAssignment assignment)
    {
        return assignment.Assignment.SubjectCourse.Associations
            .OfType<GithubSubjectCourseAssociation>()
            .First()
            .GithubOrganizationName;
    }

    public static Mentor GetMentor(this GroupAssignment assignment)
    {
        return assignment.Assignment.SubjectCourse.Mentors.First();
    }
}