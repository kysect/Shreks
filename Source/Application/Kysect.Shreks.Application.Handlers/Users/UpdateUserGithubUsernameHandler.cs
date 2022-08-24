using AutoMapper;
using Kysect.Shreks.Application.Dto.Users;
using Kysect.Shreks.Core.Users;
using Kysect.Shreks.DataAccess.Abstractions;
using Kysect.Shreks.DataAccess.Abstractions.Extensions;
using MediatR;

using static Kysect.Shreks.Application.Abstractions.Students.Commands.UpdateUserGithubUsername;

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
        user.GithubUsername = request.GithubUsername;

        _context.Users.Update(user);
        await _context.SaveChangesAsync(cancellationToken);

        var dto = _mapper.Map<UserDto>(user);

        return new Response(dto);
    }
}