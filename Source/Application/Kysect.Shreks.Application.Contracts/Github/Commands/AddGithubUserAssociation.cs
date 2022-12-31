using MediatR;

namespace Kysect.Shreks.Application.Contracts.Github.Commands;

internal class AddGithubUserAssociation
{
    public record Command(Guid UserId, string GithubUsername) : IRequest;
}