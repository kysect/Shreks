using AutoMapper;
using Kysect.Shreks.Application.Dto.Users;
using Kysect.Shreks.Common.Exceptions;
using Kysect.Shreks.Core.UserAssociations;
using Kysect.Shreks.Core.Users;
using Kysect.Shreks.DataAccess.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static Kysect.Shreks.Application.Contracts.Github.Queries.GetUserByGithubUsername;

namespace Kysect.Shreks.Application.Handlers.Github;

internal class GetUserByGithubUsernameHandler : IRequestHandler<Query, Response>
{
    private readonly IShreksDatabaseContext _context;
    private readonly IMapper _mapper;

    public GetUserByGithubUsernameHandler(IShreksDatabaseContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        User? user = await _context.UserAssociations
            .OfType<GithubUserAssociation>()
            .Where(x => x.GithubUsername.Equals(request.Username))
            .Select(x => x.User)
            .SingleOrDefaultAsync(cancellationToken);

        if (user is null)
            throw DomainInvalidOperationException.UserNotFoundByGithubUsername(request.Username);

        UserDto dto = _mapper.Map<UserDto>(user);

        return new Response(dto);
    }
}