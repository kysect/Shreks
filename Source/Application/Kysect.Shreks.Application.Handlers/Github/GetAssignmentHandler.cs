using AutoMapper;
using Kysect.CommonLib;
using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Common.Exceptions;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Core.SubjectCourseAssociations;
using Kysect.Shreks.DataAccess.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static Kysect.Shreks.Application.Contracts.Github.Queries.GetAssignment;

namespace Kysect.Shreks.Application.Handlers.Github;

internal class GetAssignmentHandler : IRequestHandler<Query, Response>
{
    private readonly IShreksDatabaseContext _context;
    private readonly IMapper _mapper;

    public GetAssignmentHandler(IShreksDatabaseContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        Assignment? assignment = await _context.SubjectCourseAssociations
            .OfType<GithubSubjectCourseAssociation>()
            .Where(x => x.GithubOrganizationName.Equals(request.OrganizationName))
            .SelectMany(x => x.SubjectCourse.Assignments)
            .Where(x => x.ShortName.Equals(request.BranchName))
            .SingleOrDefaultAsync(cancellationToken);

        if (assignment is not null)
        {
            AssignmentDto dto = _mapper.Map<AssignmentDto>(assignment);
            return new Response(dto);
        }

        SubjectCourse? subjectCourse = await _context.SubjectCourseAssociations
            .Include(x => x.SubjectCourse)
            .ThenInclude(x => x.Assignments)
            .OfType<GithubSubjectCourseAssociation>()
            .Where(x => x.GithubOrganizationName.Equals(request.OrganizationName))
            .Select(x => x.SubjectCourse)
            .SingleOrDefaultAsync(cancellationToken);

        if (subjectCourse is null)
            throw new EntityNotFoundException("SubjectCourse not found");

        string message = subjectCourse.Assignments.OrderBy(x => x.Order).ToSingleString();
        throw EntityNotFoundException.AssignmentWasNotFound(request.BranchName, subjectCourse.Title, message);
    }
}