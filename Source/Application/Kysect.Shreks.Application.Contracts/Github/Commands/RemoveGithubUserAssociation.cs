using MediatR;

namespace Kysect.Shreks.Application.Contracts.Github.Commands;

internal class RemoveGithubUserAssociation
{
    public record Command(Guid UserId) : IRequest;
}