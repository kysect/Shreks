using ITMO.Dev.ASAP.Core.UserAssociations;
using ITMO.Dev.ASAP.Core.Users;
using ITMO.Dev.ASAP.DataAccess.Abstractions;
using ITMO.Dev.ASAP.Mapping.Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static ITMO.Dev.ASAP.Application.Contracts.Users.Queries.FindUserByUniversityId;

namespace ITMO.Dev.ASAP.Application.Handlers.Users;

internal class FindUserByUniversityIdHandler : IRequestHandler<Query, Response>
{
    private readonly IDatabaseContext _context;

    public FindUserByUniversityIdHandler(IDatabaseContext context)
    {
        _context = context;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        User? user = await _context.UserAssociations
            .OfType<IsuUserAssociation>()
            .Where(x => x.UniversityId.Equals(request.UniversityId))
            .Select(x => x.User)
            .SingleOrDefaultAsync(cancellationToken);

        return new Response(user?.ToDto());
    }
}