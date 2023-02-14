using ITMO.Dev.ASAP.Application.Dto.Users;
using MediatR;

namespace ITMO.Dev.ASAP.Application.Contracts.Students.Queries;

internal static class GetStudentsBySubjectCourseId
{
    public record Query(Guid SubjectCourseId) : IRequest<Response>;

    public record Response(IReadOnlyCollection<StudentDto> Students);
}