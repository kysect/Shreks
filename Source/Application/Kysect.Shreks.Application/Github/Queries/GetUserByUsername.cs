using Kysect.Shreks.Application.Abstractions.DataAccess;
using Kysect.Shreks.Application.Exceptions;
using Kysect.Shreks.Core.UserAssociations;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Kysect.Shreks.Application.Github.Queries;

public static class GetUserByUsername
{
    public record Query(string Username) : IRequest<Response>;

    public record Response(Guid UserId);

    public class QueryHandler : IRequestHandler<Query, Response>
    {
        private IShreksDatabaseContext _context;

        public QueryHandler(IShreksDatabaseContext context)
        {
            _context = context;
        }

        public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
        {
            var association = await _context.UserAssociations
                .OfType<GithubUserAssociation>().
                FirstOrDefaultAsync(
                        gua => gua.GithubUsername == request.Username,
                        cancellationToken
                    );
            
            if (association == null)
                throw new EntityNotFoundException($"User with github username {request.Username} not found");

            return new Response(association.User.Id);
        }
    }
}