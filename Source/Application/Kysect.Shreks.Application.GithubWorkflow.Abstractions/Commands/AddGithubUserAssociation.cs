using MediatR;

namespace Kysect.Shreks.Application.GithubWorkflow.Abstractions.Commands;

public class AddGithubUserAssociation
{
    public record Command(Guid UserId, string GithubUsername) : IRequest<Response>;

    public record Response();
}