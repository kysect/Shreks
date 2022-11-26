using Kysect.Shreks.Application.GithubWorkflow.Abstractions;
using Kysect.Shreks.Common.Exceptions;
using Kysect.Shreks.Core.UserAssociations;
using Kysect.Shreks.Core.Users;
using Kysect.Shreks.DataAccess.Abstractions;
using Kysect.Shreks.DataAccess.Abstractions.Extensions;
using MediatR;
using static Kysect.Shreks.Application.Contracts.Github.Commands.AddGithubUserAssociation;

namespace Kysect.Shreks.Application.Handlers.Github;

internal class AddGithubUserAssociationHandler : IRequestHandler<Command, Response>
{
    private readonly IShreksDatabaseContext _context;
    private readonly IGithubUserProvider _githubUserProvider;

    public AddGithubUserAssociationHandler(IShreksDatabaseContext context, IGithubUserProvider githubUserProvider)
    {
        _context = context;
        _githubUserProvider = githubUserProvider;
    }

    public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
    {
        Boolean isGithubUserExists = await _githubUserProvider.IsGithubUserExists(request.GithubUsername);

        if (!isGithubUserExists)
            throw new DomainInvalidOperationException($"Github user with username {request.GithubUsername} does not exist");

        Student student = await _context.Students.GetByIdAsync(request.UserId, cancellationToken);

        var association = new GithubUserAssociation(student.User, request.GithubUsername);
        _context.UserAssociations.Add(association);
        _context.Users.Update(student.User);

        await _context.SaveChangesAsync(cancellationToken);
        
        return new Response();
    }
}