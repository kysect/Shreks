using Kysect.Shreks.Application.Dto.Users;
using MediatR;

namespace Kysect.Shreks.Application.Contracts.Users.Queries;

internal static class FindUserByUniversityId
{
    public record Query(int UniversityId) : IRequest<Response>;

    public record Response(UserDto? User);
}