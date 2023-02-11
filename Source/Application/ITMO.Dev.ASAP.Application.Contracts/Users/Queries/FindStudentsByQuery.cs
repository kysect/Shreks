using ITMO.Dev.ASAP.Application.Dto.Querying;
using ITMO.Dev.ASAP.Application.Dto.Users;
using MediatR;

namespace ITMO.Dev.ASAP.Application.Contracts.Users.Queries;

internal static class FindStudentsByQuery
{
    public record Query(QueryConfiguration<StudentQueryParameter> Configuration) : IRequest<Response>;

    public record Response(IReadOnlyCollection<StudentDto> Students);
}