using Kysect.Shreks.Core.UserAssociations;
using Kysect.Shreks.Core.Users;
using Kysect.Shreks.DataAccess.Abstractions;
using Kysect.Shreks.DataAccess.Abstractions.Extensions;
using MediatR;
using static Kysect.Shreks.Application.Contracts.Github.Commands.RemoveGithubUserAssociation;

namespace Kysect.Shreks.Application.Handlers.Github.Users;

internal class RemoveGithubUserAssociationHandler : IRequestHandler<Command>
{
    private readonly IShreksDatabaseContext _context;

    public RemoveGithubUserAssociationHandler(IShreksDatabaseContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
    {
        Student student = await _context.Students.GetByIdAsync(request.UserId, cancellationToken);

        GithubUserAssociation githubUserAssociation =
            student.User.Associations.OfType<GithubUserAssociation>().Single();
        student.User.RemoveAssociation(githubUserAssociation);
        _context.UserAssociations.Remove(githubUserAssociation);

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}