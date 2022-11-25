using Kysect.Shreks.Core.UserAssociations;
using Kysect.Shreks.Core.Users;
using Kysect.Shreks.DataAccess.Abstractions;
using Kysect.Shreks.DataAccess.Abstractions.Extensions;
using MediatR;
using static Kysect.Shreks.Application.Contracts.Github.Commands.RemoveGithubUserAssociation;

namespace Kysect.Shreks.Application.Handlers.Github;

public class RemoveGithubUserAssociationHandler : IRequestHandler<Command, Response>
{
    private readonly IShreksDatabaseContext _context;

    public RemoveGithubUserAssociationHandler(IShreksDatabaseContext context)
    {
        _context = context;
    }

    public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
    {
        Student student = await _context.Students.GetByIdAsync(request.UserId, cancellationToken);

        GithubUserAssociation githubUserAssociation = student.User.Associations.OfType<GithubUserAssociation>().Single();
        student.User.RemoveAssociation(githubUserAssociation);
        _context.UserAssociations.Remove(githubUserAssociation);

        await _context.SaveChangesAsync(cancellationToken);

        return new Response();
    }
}