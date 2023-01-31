using Kysect.CommonLib;
using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Common.Exceptions;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Core.SubjectCourseAssociations;
using Kysect.Shreks.DataAccess.Abstractions;
using Kysect.Shreks.Mapping.Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static Kysect.Shreks.Application.Contracts.Github.Queries.GetGitHubAssignment;

namespace Kysect.Shreks.Application.Handlers.Github.Assignments;

internal class GetGitHubAssignmentHandler : IRequestHandler<Query, Response>
{
    private readonly IShreksDatabaseContext _context;

    public GetGitHubAssignmentHandler(IShreksDatabaseContext context)
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