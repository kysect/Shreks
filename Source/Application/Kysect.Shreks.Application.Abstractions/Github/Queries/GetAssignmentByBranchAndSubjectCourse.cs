using MediatR;

namespace Kysect.Shreks.Application.Abstractions.Github.Queries;

public static class GetAssignmentByBranchAndSubjectCourse
{
    public record Query(string BranchName, Guid SubjectCourseId) : IRequest<Response>;

    public record Response(Guid AssignmentId);
}