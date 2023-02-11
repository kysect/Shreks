using MediatR;

namespace ITMO.Dev.ASAP.Application.Contracts.Github.Commands;

internal class RemoveGithubUserAssociation
{
    public record Command(Guid UserId) : IRequest;
}