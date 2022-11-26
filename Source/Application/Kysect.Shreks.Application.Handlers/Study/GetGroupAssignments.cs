using AutoMapper;
using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.DataAccess.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static Kysect.Shreks.Application.Contracts.Study.Queries.GetGroupAssignments;

namespace Kysect.Shreks.Application.Handlers.Study;

internal class GetGroupAssignments : IRequestHandler<Query, Response>
{
    private readonly IShreksDatabaseContext _context;
    private readonly IMapper _mapper;

    public GetGroupAssignments(IShreksDatabaseContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        List<GroupAssignment> assignments = await _context
            .GroupAssignments
            .Where(x => x.AssignmentId.Equals(request.AssignmentId))
            .ToListAsync(cancellationToken);

        GroupAssignmentDto[] dto = assignments
            .Select(_mapper.Map<GroupAssignmentDto>)
            .ToArray();

        return new Response(dto);
    }
}