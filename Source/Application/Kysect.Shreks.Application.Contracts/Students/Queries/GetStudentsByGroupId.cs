using Kysect.Shreks.Application.Dto.Users;
using MediatR;

namespace Kysect.Shreks.Application.Contracts.Students.Queries;

public static class GetStudentsByGroupId
{
    public record Query(Guid GroupId) : IRequest<Response>;

    public record Response(IReadOnlyCollection<StudentDto> Students);
}