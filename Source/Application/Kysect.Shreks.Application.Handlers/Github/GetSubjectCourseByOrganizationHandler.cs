using Kysect.Shreks.Common.Exceptions;
using Kysect.Shreks.Core.SubjectCourseAssociations;
using Kysect.Shreks.DataAccess.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static Kysect.Shreks.Application.Abstractions.Github.Queries.GetSubjectCourseByOrganization;
namespace Kysect.Shreks.Application.Handlers.Github;

public class GetSubjectCourseByOrganizationHandler : IRequestHandler<Query, Response>
{
    private readonly IShreksDatabaseContext _context;

    public GetSubjectCourseByOrganizationHandler(IShreksDatabaseContext context)
    {
        _context = context;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        var subjectCourseAssociation = await _context.SubjectCourseAssociations
            .OfType<GithubSubjectCourseAssociation>()
            .FirstOrDefaultAsync(gsc => gsc.GithubOrganizationName == request.OrganizationName, cancellationToken);

        if (subjectCourseAssociation is null)
            throw new EntityNotFoundException($"SubjectCourse with organization {request.OrganizationName} not found");

        return new Response(subjectCourseAssociation.SubjectCourse.Id);
    }
}