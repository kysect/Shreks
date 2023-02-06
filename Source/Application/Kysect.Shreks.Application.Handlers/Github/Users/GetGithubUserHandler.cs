using Kysect.Shreks.Common.Exceptions;
using Kysect.Shreks.Core.UserAssociations;
using Kysect.Shreks.Core.Users;
using Kysect.Shreks.DataAccess.Abstractions;
using Kysect.Shreks.Mapping.Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static Kysect.Shreks.Application.Contracts.Github.Queries.GetGithubUser;

namespace Kysect.Shreks.Application.Handlers.Github.Users;

internal class GetGithubUserHandler : IRequestHandler<Query, Response>
{
    private readonly IShreksDatabaseContext _context;

    public GetGithubUserHandler(IShreksDatabaseContext context)
    {
        _context = context;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        User? user = await _context.UserAssociations
            .OfType<GithubUserAssociation>()
            .Where(x => x.GithubUsername.ToLower().Equals(request.Username.ToLower()))
            .Select(x => x.User)
            .SingleOrDefaultAsync(cancellationToken);

        return user is null
            ? throw DomainInvalidOperationException.UserNotFoundByGithubUsername(request.Username)
            : new Response(user.ToDto());
    }
}