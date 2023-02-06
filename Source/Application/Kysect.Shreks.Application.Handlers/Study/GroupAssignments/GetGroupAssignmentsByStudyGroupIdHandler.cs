using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.DataAccess.Abstractions;
using Kysect.Shreks.Mapping.Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static Kysect.Shreks.Application.Contracts.Study.Queries.GetGroupAssignmentsByStudyGroupId;

namespace Kysect.Shreks.Application.Handlers.Study.GroupAssignments;

internal class GetGroupAssignmentsByStudyGroupIdHandler : IRequestHandler<Query, Response>
{
    private readonly IShreksDatabaseContext _context;

    public GetGroupAssignmentsByStudyGroupIdHandler(IShreksDatabaseContext context)
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