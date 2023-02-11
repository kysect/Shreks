using MediatR;

namespace ITMO.Dev.ASAP.Application.Contracts.Github.Commands;

internal class AddGithubUserAssociation
{
    public record Command(Guid UserId, string GithubUsername) : IRequest;
}