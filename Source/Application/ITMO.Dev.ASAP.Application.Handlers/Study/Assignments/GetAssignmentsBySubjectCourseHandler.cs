using ITMO.Dev.ASAP.Application.Dto.Study;
using ITMO.Dev.ASAP.Core.Study;
using ITMO.Dev.ASAP.DataAccess.Abstractions;
using ITMO.Dev.ASAP.Mapping.Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static ITMO.Dev.ASAP.Application.Contracts.Study.Assignments.Queries.GetAssignmentsBySubjectCourse;

namespace ITMO.Dev.ASAP.Application.Handlers.Study.Assignments;

internal class GetAssignmentsBySubjectCourseHandler : IRequestHandler<Query, Response>
{
    private readonly IDatabaseContext _context;

    public GetAssignmentsBySubjectCourseHandler(IDatabaseContext context)
    {
        _context = context;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        IReadOnlyCollection<Assignment> assignments = await _context
            .Assignments
            .Where(a => a.SubjectCourse.Id == request.SubjectCourseId)
            .ToListAsync(cancellationToken);

        AssignmentDto[] dto = assignments
            .Select(x => x.ToDto())
            .ToArray();

        return new Response(dto);
    }
}