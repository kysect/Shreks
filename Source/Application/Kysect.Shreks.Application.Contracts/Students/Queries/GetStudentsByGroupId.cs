using Kysect.Shreks.Application.Dto.Users;
using MediatR;

namespace Kysect.Shreks.Application.Contracts.Students.Queries;

internal static class GetStudentsByGroupId
{
    public record Query(Guid GroupId) : IRequest<Response>;

    public record Response(IReadOnlyCollection<StudentDto> Students);
}