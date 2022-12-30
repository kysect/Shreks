using AutoMapper;
using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Common.Exceptions;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.DataAccess.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static Kysect.Shreks.Application.Contracts.Study.Commands.UpdateGroupAssignmentDeadline;

namespace Kysect.Shreks.Application.Handlers.Study;

internal class UpdateGroupAssignmentDeadlineHandler : IRequestHandler<Command, Response>
{
    private readonly IShreksDatabaseContext _context;
    private readonly IMapper _mapper;

    public UpdateGroupAssignmentDeadlineHandler(IShreksDatabaseContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
    {
        GroupAssignment? groupAssignment = await _context
            .GroupAssignments
            .Where(groupAssignment => groupAssignment.GroupId.Equals(request.GroupId))
            .Where(groupAssignment => groupAssignment.AssignmentId.Equals(request.AssignmentId))
            .FirstOrDefaultAsync(cancellationToken);

        if (groupAssignment is null)
            throw new EntityNotFoundException("GroupAssignment not found");

        groupAssignment.Deadline = request.NewDeadline;
        await _context.SaveChangesAsync(cancellationToken);

        return new Response(_mapper.Map<GroupAssignmentDto>(groupAssignment));
    }
}