using Kysect.Shreks.Application.Contracts.Study.Assignments.Notifications;
using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Core.ValueObject;
using Kysect.Shreks.DataAccess.Abstractions;
using Kysect.Shreks.DataAccess.Abstractions.Extensions;
using Kysect.Shreks.Mapping.Mappings;
using MediatR;
using static Kysect.Shreks.Application.Contracts.Study.Assignments.Commands.UpdateAssignmentPoints;

namespace Kysect.Shreks.Application.Handlers.Study.Assignments;

internal class UpdateAssignmentPointsHandler : IRequestHandler<Command, Response>
{
    private readonly IShreksDatabaseContext _context;
    private readonly IPublisher _publisher;

    public UpdateAssignmentPointsHandler(IShreksDatabaseContext context, IPublisher publisher)
    {
        _context = context;
        _publisher = publisher;
    }

    public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
    {
        Assignment assignment = await _context.Assignments.GetByIdAsync(request.AssignmentId, cancellationToken);

        assignment.UpdateMinPoints(new Points(request.MinPoints));
        assignment.UpdateMaxPoints(new Points(request.MaxPoints));

        _context.Assignments.Update(assignment);
        await _context.SaveChangesAsync(cancellationToken);

        AssignmentDto dto = assignment.ToDto();

        var notification = new AssignmentPointsUpdated.Notification(dto);
        await _publisher.Publish(notification, cancellationToken);

        return new Response(dto);
    }
}