using ITMO.Dev.ASAP.Application.Dto.Study;
using ITMO.Dev.ASAP.Core.Study;
using ITMO.Dev.ASAP.DataAccess.Abstractions;
using ITMO.Dev.ASAP.Mapping.Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static ITMO.Dev.ASAP.Application.Contracts.Study.GroupAssignments.Queries.GetGroupAssignmentsByStudyGroupId;

namespace ITMO.Dev.ASAP.Application.Handlers.Study.GroupAssignments;

internal class GetGroupAssignmentsByStudyGroupIdHandler : IRequestHandler<Query, Response>
{
    private readonly IDatabaseContext _context;

    public GetGroupAssignmentsByStudyGroupIdHandler(IDatabaseContext context)
    {
        _context = context;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        IReadOnlyCollection<GroupAssignment> groupAssignments = await _context
            .GroupAssignments
            .Where(groupAssignment => groupAssignment.GroupId.Equals(request.GroupId))
            .ToListAsync(cancellationToken);

        GroupAssignmentDto[] dto = groupAssignments
            .Select(x => x.ToDto())
            .ToArray();

        return new Response(dto);
    }
}