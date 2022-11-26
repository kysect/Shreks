using AutoMapper;
using Kysect.Shreks.Application.Dto.Users;
using Kysect.Shreks.Core.UserAssociations;
using Kysect.Shreks.Core.Users;
using Kysect.Shreks.DataAccess.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static Kysect.Shreks.Application.Contracts.Users.Queries.FindUserByUniversityId;

namespace Kysect.Shreks.Application.Handlers.Users;

internal class FindUserByUniversityIdHandler : IRequestHandler<Query, Response>
{
    private readonly IShreksDatabaseContext _context;
    private readonly IMapper _mapper;

    public FindUserByUniversityIdHandler(IShreksDatabaseContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        User? user = await _context.UserAssociations
            .OfType<IsuUserAssociation>()
            .Where(x => x.UniversityId.Equals(request.UniversityId))
            .Select(x => x.User)
            .SingleOrDefaultAsync(cancellationToken);

        if (user is null)
            return new Response(null);

        UserDto userDto = _mapper.Map<UserDto>(user);
        return new Response(userDto);
    }
}