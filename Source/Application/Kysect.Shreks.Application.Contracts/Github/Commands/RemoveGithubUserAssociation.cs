using MediatR;

namespace Kysect.Shreks.Application.Contracts.Github.Commands;

public class RemoveGithubUserAssociation
{
    public record Command(Guid UserId) : IRequest<Response>;

    public record Response();
}