using Kysect.Shreks.Application.Abstractions.DataAccess;
using Kysect.Shreks.Application.Common.Exceptions;
using Kysect.Shreks.Core.SubjectCourseAssociations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static Kysect.Shreks.Application.Abstractions.Github.Queries.GetSubjectCourseByOrganization;
namespace Kysect.Shreks.Application.Handlers.Github;

public class GetSubjectCourseByOrganisationHandler : IRequestHandler<Query, Response>
{
    private readonly IShreksDatabaseContext _context;

    public GetSubjectCourseByOrganisationHandler(IShreksDatabaseContext context)
    {
        _context = context;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        var subjectCourseAssociation = await _context.SubjectCourseAssociations
            .OfType<GithubSubjectCourseAssociation>()
            .FirstOrDefaultAsync(gsc => gsc.GithubOrganizationName == request.OrganizationName, cancellationToken);

        if (subjectCourseAssociation is null)
            throw new EntityNotFoundException($"SubjectCourse with organisation {request.OrganizationName} not found");

        return new Response(subjectCourseAssociation.SubjectCourse.Id);
    }
}