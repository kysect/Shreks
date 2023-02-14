using ITMO.Dev.ASAP.Application.Dto.Study;
using MediatR;

namespace ITMO.Dev.ASAP.Application.Contracts.Study.Assignments.Queries;

internal static class GetAssignmentsBySubjectCourse
{
    public record Query(Guid SubjectCourseId) : IRequest<Response>;

    public record Response(IReadOnlyCollection<AssignmentDto> Assignments);
}