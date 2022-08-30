using AutoMapper;
using Kysect.Shreks.Application.Dto.Users;
using Kysect.Shreks.Core.UserAssociations;
using Kysect.Shreks.Core.Users;
using Kysect.Shreks.DataAccess.Abstractions;
using Kysect.Shreks.DataAccess.Abstractions.Extensions;
using MediatR;

using static Kysect.Shreks.Application.Abstractions.Users.Commands.UpdateUserGithubUsername;

namespace Kysect.Shreks.Application.Handlers.Users;

public class UpdateUserGithubUsernameHandler : IRequestHandler<Command, Response>
{
    private readonly IShreksDatabaseContext _context;
    private readonly IMapper _mapper;

    public UpdateUserGithubUsernameHandler(IShreksDatabaseContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
    {
        User user = await _context.Users.GetByIdAsync(request.UserId, cancellationToken);

        // TODO: validate that github username was not used
        var association = new GithubUserAssociation(user, request.GithubUsername);
        user.AddAssociation(association);

        await _context.UserAssociations.AddAsync(association, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);


        var dto = _mapper.Map<UserDto>(user);

        return new Response(dto);
    }
}