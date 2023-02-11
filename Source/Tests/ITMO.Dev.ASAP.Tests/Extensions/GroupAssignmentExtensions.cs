using ITMO.Dev.ASAP.Core.Models;
using ITMO.Dev.ASAP.Core.Study;
using ITMO.Dev.ASAP.Core.SubjectCourseAssociations;
using ITMO.Dev.ASAP.Core.Users;

namespace ITMO.Dev.ASAP.Tests.Extensions;

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