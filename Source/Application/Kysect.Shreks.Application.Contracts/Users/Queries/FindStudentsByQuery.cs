using Kysect.Shreks.Application.Dto.Querying;
using Kysect.Shreks.Application.Dto.Users;
using MediatR;

namespace Kysect.Shreks.Application.Contracts.Users.Queries;

public static class FindStudentsByQuery
{
    public record Query(QueryConfiguration<StudentQueryParameter> Configuration) : IRequest<Response>;

    public record Response(IReadOnlyCollection<StudentDto> Students);
}