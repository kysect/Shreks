using Kysect.Shreks.Core.UserAssociations;
using Kysect.Shreks.Core.Users;
using Kysect.Shreks.DataAccess.Abstractions;
using Kysect.Shreks.Mapping.Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static Kysect.Shreks.Application.Contracts.Users.Queries.FindUserByUniversityId;

namespace Kysect.Shreks.Application.Handlers.Users;

internal class FindUserByUniversityIdHandler : IRequestHandler<Query, Response>
{
    private readonly IShreksDatabaseContext _context;

    public FindUserByUniversityIdHandler(IShreksDatabaseContext context)
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