using Kysect.Shreks.Common.Resources;

namespace Kysect.Shreks.Common.Exceptions;

public class StudentAssignmentException : ShreksDomainException
{
    private StudentAssignmentException(string message)
        : base(message) { }

    public static StudentAssignmentException StudentGroupAssignmentMismatch(string student, string group)
    {
        string message = string.Format(UserMessages.StudentGroupAssignmentMismatch, student, group);
        return new StudentAssignmentException(message);
    }
}