using ITMO.Dev.ASAP.Common.Exceptions;
using ITMO.Dev.ASAP.Core.Study;
using ITMO.Dev.ASAP.Core.SubjectCourseAssociations;
using ITMO.Dev.ASAP.DataAccess.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static ITMO.Dev.ASAP.Application.Contracts.Github.Queries.GetGitHubSubjectCourseId;

namespace ITMO.Dev.ASAP.Application.Handlers.Github.SubjectCourses;

internal class GetSubjectCourseIdHandler : IRequestHandler<Query, Response>
{
    private readonly IDatabaseContext _context;

    public GetSubjectCourseIdHandler(IDatabaseContext context)
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