using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Core.Users;

namespace Kysect.Shreks.GoogleIntegration.Extensions.Entities;

internal static class SubmissionExtensions
{
    public static IList<object> ToSheetData(this Submission submission)
    {
        StudentAssignment assignment = submission.StudentAssignment;
        Student student = assignment.Student;

        return new List<object>
        {
            student.GetFullName(),
            student.Group.Name,
            //TODO: change to short name
            assignment.Assignment.Title,
            //TODO: change to real pr link
            "https://github.com/kysect/Shreks/pull/29"
        };
    }
}