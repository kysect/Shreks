using MediatR;

namespace Kysect.Shreks.Application.Abstractions.Github.Commands;

public class AddGithubUserAssociation
{
    public record Command(Guid UserId, string GithubUsername) : IRequest<Response>;

    public record Response();
}