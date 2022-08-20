using Kysect.Shreks.Application.Abstractions.Exceptions;
using Kysect.Shreks.Core.UserAssociations;
using Kysect.Shreks.DataAccess.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static Kysect.Shreks.Application.Abstractions.Github.Queries.GetUserByUsername;
namespace Kysect.Shreks.Application.Handlers.Github;

public class GetUserByUsernameHandler : IRequestHandler<Query, Response>
{
    private readonly IShreksDatabaseContext _context;

    public GetUserByUsernameHandler(IShreksDatabaseContext context)
    {
        _context = context;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
         var userAssociation = await _context.UserAssociations
             .OfType<GithubUserAssociation>()
             .FirstOrDefaultAsync(gua => gua.GithubUsername == request.Username, cancellationToken);

        if (userAssociation is null)
            throw new EntityNotFoundException($"User with github username {request.Username} not found");

        return new Response(userAssociation.User.Id);
    }
}