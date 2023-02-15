using ITMO.Dev.ASAP.Application.Dto.Study;
using ITMO.Dev.ASAP.Common.Exceptions;
using ITMO.Dev.ASAP.Core.Study;
using ITMO.Dev.ASAP.Core.SubjectCourseAssociations;
using ITMO.Dev.ASAP.DataAccess.Abstractions;
using ITMO.Dev.ASAP.Mapping.Mappings;
using Kysect.CommonLib;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static ITMO.Dev.ASAP.Application.Contracts.Github.Queries.GetGitHubAssignment;

namespace ITMO.Dev.ASAP.Application.Handlers.Github.Assignments;

internal class GetGitHubAssignmentHandler : IRequestHandler<Query, Response>
{
    private readonly IDatabaseContext _context;

    public GetGitHubAssignmentHandler(IDatabaseContext context)
    {
        _context = context;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        Assignment? assignment = await _context.SubjectCourseAssociations
            .OfType<GithubSubjectCourseAssociation>()
            .Where(x => x.GithubOrganizationName.ToLower().Equals(request.OrganizationName.ToLower()))
            .SelectMany(x => x.SubjectCourse.Assignments)
            .Where(x => x.ShortName.ToLower().Equals(request.BranchName.ToLower()))
            .SingleOrDefaultAsync(cancellationToken);

        if (assignment is not null)
        {
            AssignmentDto dto = assignment.ToDto();
            return new Response(dto);
        }

        SubjectCourse? subjectCourse = await _context.SubjectCourseAssociations
            .Include(x => x.SubjectCourse)
            .ThenInclude(x => x.Assignments)
            .OfType<GithubSubjectCourseAssociation>()
            .Where(x => x.GithubOrganizationName.ToLower().Equals(request.OrganizationName.ToLower()))
            .Select(x => x.SubjectCourse)
            .SingleOrDefaultAsync(cancellationToken);

        if (subjectCourse is null)
            throw new EntityNotFoundException("SubjectCourse not found");

        string message = subjectCourse.Assignments.OrderBy(x => x.Order).ToSingleString();
        throw EntityNotFoundException.AssignmentWasNotFound(request.BranchName, subjectCourse.Title, message);
    }
}