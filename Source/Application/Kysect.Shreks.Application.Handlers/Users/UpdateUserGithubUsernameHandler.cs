using AutoMapper;
using Kysect.Shreks.Application.Dto.Users;
using Kysect.Shreks.Application.GithubWorkflow.Abstractions.Providers;
using Kysect.Shreks.Common.Exceptions;
using Kysect.Shreks.Core.UserAssociations;
using Kysect.Shreks.Core.Users;
using Kysect.Shreks.DataAccess.Abstractions;
using Kysect.Shreks.DataAccess.Abstractions.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static Kysect.Shreks.Application.Contracts.Users.Commands.UpdateUserGithubUsername;

namespace Kysect.Shreks.Application.Handlers.Users;

internal class UpdateUserGithubUsernameHandler : IRequestHandler<Command, Response>
{
    private readonly IShreksDatabaseContext _context;
    private readonly IMapper _mapper;
    private readonly IGithubUserProvider _githubUserProvider;

    public UpdateUserGithubUsernameHandler(IShreksDatabaseContext context, IMapper mapper, IGithubUserProvider githubUserProvider)
    {
        _context = context;
        _mapper = mapper;
        _githubUserProvider = githubUserProvider;
    }

    public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
    {
        User user = await _context.Users.GetByIdAsync(request.UserId, cancellationToken);

        bool usernameAlreadyExists = await _context
            .UserAssociations
            .OfType<GithubUserAssociation>()
            .AnyAsync(a => a.GithubUsername == request.GithubUsername, cancellationToken: cancellationToken);

        if (usernameAlreadyExists)
            throw new DomainInvalidOperationException($"Username {request.GithubUsername} already used by other user");

        bool isGithubUserExists = await _githubUserProvider.IsGithubUserExists(request.GithubUsername);

        if (!isGithubUserExists)
            throw new DomainInvalidOperationException($"Github user with username {request.GithubUsername} does not exist");

        var association = new GithubUserAssociation(user, request.GithubUsername);
        user.AddAssociation(association);

        await _context.UserAssociations.AddAsync(association, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);


        UserDto? dto = _mapper.Map<UserDto>(user);

        return new Response(dto);
    }
}