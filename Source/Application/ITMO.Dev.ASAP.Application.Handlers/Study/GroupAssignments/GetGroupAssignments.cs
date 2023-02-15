using ITMO.Dev.ASAP.Application.Dto.Study;
using ITMO.Dev.ASAP.Core.Study;
using ITMO.Dev.ASAP.DataAccess.Abstractions;
using ITMO.Dev.ASAP.Mapping.Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static ITMO.Dev.ASAP.Application.Contracts.Study.GroupAssignments.Queries.GetGroupAssignments;

namespace ITMO.Dev.ASAP.Application.Handlers.Study.GroupAssignments;

internal class GetGroupAssignments : IRequestHandler<Query, Response>
{
    private readonly IDatabaseContext _context;

    public GetGroupAssignments(IDatabaseContext context)
    {
        _context = context;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        List<GroupAssignment> assignments = await _context
            .GroupAssignments
            .Where(x => x.AssignmentId.Equals(request.AssignmentId))
            .ToListAsync(cancellationToken);

        GroupAssignmentDto[] dto = assignments
            .Select(x => x.ToDto())
            .ToArray();

        return new Response(dto);
    }
}