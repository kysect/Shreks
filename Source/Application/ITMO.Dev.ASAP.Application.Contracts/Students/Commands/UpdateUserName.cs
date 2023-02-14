using MediatR;

namespace ITMO.Dev.ASAP.Application.Contracts.Students.Commands;

internal static class UpdateUserName
{
    public record Command(Guid UserId, string FirstName, string MiddleName, string LastName) : IRequest<Response>;

    public record Response;
}