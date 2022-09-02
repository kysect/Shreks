using Kysect.Shreks.Application.Dto.Users;
using MediatR;

namespace Kysect.Shreks.Application.Abstractions.Students;

public static class GetStudentsByGroupId
{
    public record Query(Guid GroupId) : IRequest<Response>;

    public record Response(IReadOnlyCollection<StudentDto> Students);
}