using Kysect.Shreks.Application.GithubWorkflow.Abstractions.Providers;
using Kysect.Shreks.Common.Exceptions;
using Kysect.Shreks.Core.UserAssociations;
using Kysect.Shreks.Core.Users;
using Kysect.Shreks.DataAccess.Abstractions;
using Kysect.Shreks.DataAccess.Abstractions.Extensions;
using MediatR;
using static Kysect.Shreks.Application.Contracts.Github.Commands.AddGithubUserAssociation;

namespace Kysect.Shreks.Application.Handlers.Github;

internal class AddGithubUserAssociationHandler : IRequestHandler<Command>
{
    private readonly IShreksDatabaseContext _context;
    private readonly IGithubUserProvider _githubUserProvider;

    public AddGithubUserAssociationHandler(IShreksDatabaseContext context, IGithubUserProvider githubUserProvider)
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