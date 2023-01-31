using Kysect.Shreks.Common.Exceptions;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Core.SubjectCourseAssociations;
using Kysect.Shreks.DataAccess.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static Kysect.Shreks.Application.Contracts.Github.Queries.GetGitHubSubjectCourseId;

namespace Kysect.Shreks.Application.Handlers.Github.SubjectCourses;

internal class GetSubjectCourseIdHandler : IRequestHandler<Query, Response>
{
    private readonly IShreksDatabaseContext _context;

    public GetSubjectCourseIdHandler(IShreksDatabaseContext context)
    {
        _context = context;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        SubjectCourse? subjectCourse = await _context.SubjectCourseAssociations
            .OfType<GithubSubjectCourseAssociation>()
            .Where(x => x.GithubOrganizationName.ToLower().Equals(request.OrganizationName.ToLower()))
            .Select(x => x.SubjectCourse)
            .SingleOrDefaultAsync(cancellationToken);

        return subjectCourse is null
            ? throw new EntityNotFoundException("SubjectCourse not found")
            : new Response(subjectCourse.Id);
    }
}