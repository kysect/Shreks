using AutoMapper;
using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.DataAccess.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static Kysect.Shreks.Application.Abstractions.Study.Queries.GetGroupAssignmentsByStudyGroupId;

namespace Kysect.Shreks.Application.Handlers.Study;

public class GetGroupAssignmentsByStudyGroupIdHandler : IRequestHandler<Query, Response>
{
    private readonly IShreksDatabaseContext _context;
    private readonly IMapper _mapper;

    public GetGroupAssignmentsByStudyGroupIdHandler(IShreksDatabaseContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        IReadOnlyCollection<GroupAssignment> groupAssignments = await _context
            .GroupAssignments
            .Where(groupAssignment => groupAssignment.GroupId.Equals(request.GroupId))
            .ToListAsync(cancellationToken);

        return new Response(_mapper.Map<IReadOnlyCollection<GroupAssignmentDto>>(groupAssignments));
    }
}