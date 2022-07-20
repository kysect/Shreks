using Kysect.Shreks.Application.Abstractions.DataAccess;
using Kysect.Shreks.Application.Exceptions;
using Kysect.Shreks.Core.SubjectCourseAssociations;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Kysect.Shreks.Application.Github.Queries;

public static class GetSubjectCourseByOrganization
{
    public record Query(string OrganizationName) : IRequest<Response>;

    public record Response(Guid SubjectCourseId);

    public class QueryHandler : IRequestHandler<Query, Response>
    {
        private IShreksDatabaseContext _context;

        public QueryHandler(IShreksDatabaseContext context)
        {
            _context = context;
        }

        public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
        {
            var association = await _context.SubjectCourseAssociations
                .OfType<GithubSubjectCourseAssociation>()
                .FirstOrDefaultAsync(
                    gsc => gsc.GithubOrganizationName == request.OrganizationName,
                    cancellationToken
                );
            
            if (association is null)
                throw new EntityNotFoundException($"Subject course with github organisation {request.OrganizationName} not found");
            
            return new Response(association.SubjectCourse.Id);
        }
    }
}