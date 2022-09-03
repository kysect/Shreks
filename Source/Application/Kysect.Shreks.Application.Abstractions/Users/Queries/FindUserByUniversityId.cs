using Kysect.Shreks.Application.Dto.Users;
using MediatR;

namespace Kysect.Shreks.Application.Abstractions.Users.Queries;

public static class FindUserByUniversityId
{
    public record Query(int UniversityId) : IRequest<Response>;

    public record Response(UserDto? User);
}