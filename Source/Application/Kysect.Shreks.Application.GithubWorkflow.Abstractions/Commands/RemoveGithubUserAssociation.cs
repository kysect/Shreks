using MediatR;

namespace Kysect.Shreks.Application.GithubWorkflow.Abstractions.Commands;

public class RemoveGithubUserAssociation
{
    public record Command(Guid UserId) : IRequest<Response>;

    public record Response();
}