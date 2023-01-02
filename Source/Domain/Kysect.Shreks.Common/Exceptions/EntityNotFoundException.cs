using Kysect.Shreks.Common.Resources;

namespace Kysect.Shreks.Common.Exceptions;

public class EntityNotFoundException : NotFoundException
{
    public EntityNotFoundException(string? message) : base(message) { }

    public static EntityNotFoundException For<T>(Guid id)
    {
        return new EntityNotFoundException($"Entity of type {typeof(T).Name} with id {id} was not found");
    }

    public static EntityNotFoundException AssignmentWasNotFound(string branchName, string subjectCourseTitle,
        string subjectCourseAssignments)
    {
        return new EntityNotFoundException(string.Format(UserMessages.AssignmentNotFound, branchName,
            subjectCourseTitle, subjectCourseAssignments));
    }

    public static EntityNotFoundException SubjectCourseForOrganizationNotFound(string organization)
    {
        return new EntityNotFoundException(string.Format(UserMessages.SubjectCourseForOrganizationNotFound,
            organization));
    }

    public static EntityNotFoundException UserNotFoundInSubjectCourse(Guid userId, string subjectCourse)
    {
        return new EntityNotFoundException(string.Format(UserMessages.UserNotFoundInSubjectCourse, userId,
            subjectCourse));
    }

    public static EntityNotFoundException NoUnratedSubmissionInPullRequest(string payload)
    {
        return new EntityNotFoundException(string.Format(UserMessages.NoUnratedSubmissionInPullRequest, payload));
    }

    public static EntityNotFoundException NoSubmissionInPullRequest(string payload)
    {
        return new EntityNotFoundException(string.Format(UserMessages.NoSubmissionInPullRequest, payload));
    }
}