using Kysect.Shreks.Application.Abstractions.DataAccess;
using Kysect.Shreks.Application.Common.Exceptions;
using Kysect.Shreks.Core.UserAssociations;
using MediatR;
using static Kysect.Shreks.Application.Abstractions.Github.Queries.GetUserByUsername;
namespace Kysect.Shreks.Application.Handlers.Github;

public class GetUserByUsernameHandler : IRequestHandler<Query, Response>
{
    private readonly IShreksDatabaseContext _context;

    public GetUserByUsernameHandler(IShreksDatabaseContext context)
    {
        _context = context;
    }

    public Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
         var userAssociation = _context.UserAssociations.OfType<GithubUserAssociation>()
            .FirstOrDefault(gua => gua.GithubUsername == request.Username);

        if (userAssociation is null)
            throw new EntityNotFoundException($"User with github username {request.Username} not found");

        return Task.FromResult(new Response(userAssociation.User.Id));
    }
}