using ITMO.Dev.ASAP.Application.GithubWorkflow.Abstractions.Providers;
using ITMO.Dev.ASAP.Common.Exceptions;
using ITMO.Dev.ASAP.Core.UserAssociations;
using ITMO.Dev.ASAP.Core.Users;
using ITMO.Dev.ASAP.DataAccess.Abstractions;
using ITMO.Dev.ASAP.DataAccess.Abstractions.Extensions;
using MediatR;
using static ITMO.Dev.ASAP.Application.Contracts.Github.Commands.AddGithubUserAssociation;

namespace ITMO.Dev.ASAP.Application.Handlers.Github.Users;

internal class AddGithubUserAssociationHandler : IRequestHandler<Command>
{
    private readonly IDatabaseContext _context;
    private readonly IGithubUserProvider _githubUserProvider;

    public AddGithubUserAssociationHandler(IDatabaseContext context, IGithubUserProvider githubUserProvider)
    {
        _context = context;
        _githubUserProvider = githubUserProvider;
    }

    public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
    {
        bool isGithubUserExists = await _githubUserProvider.IsGithubUserExists(request.GithubUsername);

        if (!isGithubUserExists)
        {
            string message = $"Github user with username {request.GithubUsername} does not exist";
            throw new DomainInvalidOperationException(message);
        }

        Student student = await _context.Students.GetByIdAsync(request.UserId, cancellationToken);

        var association = new GithubUserAssociation(Guid.NewGuid(), student.User, request.GithubUsername);
        _context.UserAssociations.Add(association);
        _context.Users.Update(student.User);

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}