using AutoMapper;
using Kysect.Shreks.Core.UserAssociations;
using Kysect.Shreks.Core.Users;
using Kysect.Shreks.DataAccess.Abstractions;
using Kysect.Shreks.DataAccess.Abstractions.Extensions;
using MediatR;
using static Kysect.Shreks.Application.Abstractions.Github.Commands.AddGithubUserAssociation;

namespace Kysect.Shreks.Application.Handlers.Github;

public class AddGithubUserAssociationHandler : IRequestHandler<Command, Response>
{
    private readonly IShreksDatabaseContext _context;

    public AddGithubUserAssociationHandler(IShreksDatabaseContext context, IMapper mapper)
    {
        _context = context;
    }

    public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
    {
        Student student = await _context.Students.GetByIdAsync(request.UserId, cancellationToken);

        var association = new GithubUserAssociation(student.User, request.GithubUsername);
        _context.UserAssociations.Add(association);
        _context.Users.Update(student.User);

        await _context.SaveChangesAsync(cancellationToken);
        return new Response();
    }
}