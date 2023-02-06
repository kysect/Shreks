using Kysect.Shreks.Application.Contracts.Study.GroupAssignments.Notifications;
using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Common.Exceptions;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.DataAccess.Abstractions;
using Kysect.Shreks.Mapping.Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static Kysect.Shreks.Application.Contracts.Study.GroupAssignments.Commands.UpdateGroupAssignmentDeadline;

namespace Kysect.Shreks.Application.Handlers.Study.GroupAssignments;

internal class UpdateGroupAssignmentDeadlineHandler : IRequestHandler<Command, Response>
{
    private readonly IShreksDatabaseContext _context;
    private readonly IPublisher _publisher;

    public UpdateGroupAssignmentDeadlineHandler(IShreksDatabaseContext context, IPublisher publisher)
    {
        _context = context;
        _publisher = publisher;
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

        GroupAssignmentDto dto = groupAssignment.ToDto();

        var notification = new GroupAssignmentDeadlineUpdated.Notification(dto);
        await _publisher.PublishAsync(notification, cancellationToken);

        return new Response(dto);
    }
}