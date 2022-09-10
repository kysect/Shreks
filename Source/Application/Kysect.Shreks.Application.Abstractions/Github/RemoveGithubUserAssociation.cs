using MediatR;

namespace Kysect.Shreks.Application.Abstractions.Github;

public class RemoveGithubUserAssociation
{
    public record Command(Guid UserId) : IRequest<Response>;

    public record Response();
}