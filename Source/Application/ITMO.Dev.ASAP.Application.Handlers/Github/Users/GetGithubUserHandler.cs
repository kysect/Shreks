using ITMO.Dev.ASAP.Common.Exceptions;
using ITMO.Dev.ASAP.Core.UserAssociations;
using ITMO.Dev.ASAP.Core.Users;
using ITMO.Dev.ASAP.DataAccess.Abstractions;
using ITMO.Dev.ASAP.Mapping.Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static ITMO.Dev.ASAP.Application.Contracts.Github.Queries.GetGithubUser;

namespace ITMO.Dev.ASAP.Application.Handlers.Github.Users;

internal class GetGithubUserHandler : IRequestHandler<Query, Response>
{
    private readonly IDatabaseContext _context;

    public GetGithubUserHandler(IDatabaseContext context)
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