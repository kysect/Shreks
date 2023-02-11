using ITMO.Dev.ASAP.Common.Resources;

namespace ITMO.Dev.ASAP.Common.Exceptions;

public class StudentAssignmentException : DomainException
{
    private StudentAssignmentException(string message) : base(message) { }

    public static StudentAssignmentException StudentGroupAssignmentMismatch(string student, string group)
    {
        string message = string.Format(UserMessages.StudentGroupAssignmentMismatch, student, group);
        return new StudentAssignmentException(message);
    }
}