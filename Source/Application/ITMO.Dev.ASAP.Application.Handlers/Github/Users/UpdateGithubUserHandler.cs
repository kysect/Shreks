using ITMO.Dev.ASAP.Application.GithubWorkflow.Abstractions.Providers;
using ITMO.Dev.ASAP.Common.Exceptions;
using ITMO.Dev.ASAP.Core.UserAssociations;
using ITMO.Dev.ASAP.Core.Users;
using ITMO.Dev.ASAP.DataAccess.Abstractions;
using ITMO.Dev.ASAP.DataAccess.Abstractions.Extensions;
using ITMO.Dev.ASAP.Mapping.Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static ITMO.Dev.ASAP.Application.Contracts.Users.Commands.UpdateGithubUser;

namespace ITMO.Dev.ASAP.Application.Handlers.Github.Users;

internal class UpdateGithubUserHandler : IRequestHandler<Command, Response>
{
    private readonly IDatabaseContext _context;
    private readonly IGithubUserProvider _githubUserProvider;

    public UpdateGithubUserHandler(
        IDatabaseContext context,
        IGithubUserProvider githubUserProvider)
    {
        _context = context;
        _githubUserProvider = githubUserProvider;
    }

    public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
    {
        User user = await _context.Users.GetByIdAsync(request.UserId, cancellationToken);

        bool usernameAlreadyExists = await _context
            .UserAssociations
            .OfType<GithubUserAssociation>()
            .AnyAsync(a => a.GithubUsername == request.GithubUsername, cancellationToken);

        if (usernameAlreadyExists)
            throw new DomainInvalidOperationException($"Username {request.GithubUsername} already used by other user");

        bool isGithubUserExists = await _githubUserProvider.IsGithubUserExists(request.GithubUsername);

        if (!isGithubUserExists)
        {
            string message = $"Github user with username {request.GithubUsername} does not exist";
            throw new DomainInvalidOperationException(message);
        }

        var association = new GithubUserAssociation(Guid.NewGuid(), user, request.GithubUsername);
        user.AddAssociation(association);

        await _context.UserAssociations.AddAsync(association, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return new Response(user.ToDto());
    }
}