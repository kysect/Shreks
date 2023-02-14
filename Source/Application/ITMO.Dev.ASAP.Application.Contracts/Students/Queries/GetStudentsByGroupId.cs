using ITMO.Dev.ASAP.Application.Dto.Users;
using MediatR;

namespace ITMO.Dev.ASAP.Application.Contracts.Students.Queries;

internal static class GetStudentsByGroupId
{
    public record Query(Guid GroupId) : IRequest<Response>;

    public record Response(IReadOnlyCollection<StudentDto> Students);
}