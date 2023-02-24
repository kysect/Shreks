using ITMO.Dev.ASAP.Core.UserAssociations;
using ITMO.Dev.ASAP.Core.Users;
using ITMO.Dev.ASAP.DataAccess.Abstractions;
using ITMO.Dev.ASAP.DataAccess.Abstractions.Extensions;
using MediatR;
using static ITMO.Dev.ASAP.Application.Contracts.Github.Commands.RemoveGithubUserAssociation;

namespace ITMO.Dev.ASAP.Application.Handlers.Github.Users;

internal class RemoveGithubUserAssociationHandler : IRequestHandler<Command>
{
    private readonly IDatabaseContext _context;

    public RemoveGithubUserAssociationHandler(IDatabaseContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
    {
        Student student = await _context.Students.GetByIdAsync(request.UserId, cancellationToken);

        GithubUserAssociation? githubUserAssociation = student.User.FindAssociation<GithubUserAssociation>();

        if (githubUserAssociation is not null)
        {
            student.User.RemoveAssociation(githubUserAssociation);
            _context.UserAssociations.Remove(githubUserAssociation);

            await _context.SaveChangesAsync(cancellationToken);
        }

        return Unit.Value;
    }
}