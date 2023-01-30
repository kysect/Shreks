using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Core.ValueObject;
using Kysect.Shreks.DataAccess.Abstractions;
using Kysect.Shreks.DataAccess.Abstractions.Extensions;
using Kysect.Shreks.Mapping.Mappings;
using MediatR;
using static Kysect.Shreks.Application.Contracts.Study.Commands.UpdateAssignmentPoints;

namespace Kysect.Shreks.Application.Handlers.Study.Assignments;

internal class UpdateAssignmentPointsHandler : IRequestHandler<Command, Response>
{
    private readonly IShreksDatabaseContext _context;

    public UpdateAssignmentPointsHandler(IShreksDatabaseContext context)
    {
        _context = context;
    }

    public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
    {
        Assignment assignment = await _context.Assignments.GetByIdAsync(request.AssignmentId, cancellationToken);

        assignment.UpdateMinPoints(new Points(request.MinPoints));
        assignment.UpdateMaxPoints(new Points(request.MaxPoints));

        _context.Assignments.Update(assignment);
        await _context.SaveChangesAsync(cancellationToken);

        return new Response(assignment.ToDto());
    }
}