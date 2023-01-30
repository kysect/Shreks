using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.DataAccess.Abstractions;
using Kysect.Shreks.Mapping.Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static Kysect.Shreks.Application.Contracts.Study.Queries.GetGroupAssignments;

namespace Kysect.Shreks.Application.Handlers.Study;

internal class GetGroupAssignments : IRequestHandler<Query, Response>
{
    private readonly IShreksDatabaseContext _context;

    public GetGroupAssignments(IShreksDatabaseContext context)
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