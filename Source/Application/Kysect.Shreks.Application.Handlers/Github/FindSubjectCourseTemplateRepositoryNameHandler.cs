using Kysect.Shreks.Core.SubjectCourseAssociations;
using Kysect.Shreks.DataAccess.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static Kysect.Shreks.Application.Abstractions.Github.Queries.FindSubjectCourseTemplateRepositoryName;

namespace Kysect.Shreks.Application.Handlers.Github;

public class FindSubjectCourseTemplateRepositoryNameHandler : IRequestHandler<Query, Response>
{
    private readonly IShreksDatabaseContext _context;

    public FindSubjectCourseTemplateRepositoryNameHandler(IShreksDatabaseContext context)
    {
        _context = context;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        var subjectCourseAssociation = await _context.SubjectCourseAssociations
            .OfType<GithubSubjectCourseAssociation>()
            .FirstOrDefaultAsync(gsc => gsc.GithubOrganizationName == request.OrganizationName, cancellationToken);

        if (subjectCourseAssociation is not null)
            return new Response(subjectCourseAssociation.TemplateRepositoryName);

        return new Response(null);
    }
}