using Kysect.Shreks.Core.Exceptions;
using Kysect.Shreks.Core.Extensions;
using Kysect.Shreks.Core.Specifications.Github;
using Kysect.Shreks.Core.Users;
using Kysect.Shreks.DataAccess.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

using static Kysect.Shreks.Application.Abstractions.Github.Queries.GetUserByGithubUsername;

namespace Kysect.Shreks.Application.Handlers.Github;

public class GetUserByUsernameHandler : IRequestHandler<Query, Response>
{
    private readonly IShreksDatabaseContext _context;

    public GetUserByUsernameHandler(IShreksDatabaseContext context)
    {
        _context = context;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        string userGithubUsername = request.Username;
        User? user = await _context.UserAssociations
            .WithSpecification(new FindUserByGithubUsernameSpec(userGithubUsername))
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);

        if (user is null)
            throw new UserWasNotFoundByGithubUsernameException(request.Username);
        
        return new Response(user.Id);
    }
}