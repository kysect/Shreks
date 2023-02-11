using ITMO.Dev.ASAP.Application.Dto.Users;
using MediatR;

namespace ITMO.Dev.ASAP.Application.Contracts.Users.Queries;

internal static class FindUserByUniversityId
{
    public record Query(int UniversityId) : IRequest<Response>;

    public record Response(UserDto? User);
}