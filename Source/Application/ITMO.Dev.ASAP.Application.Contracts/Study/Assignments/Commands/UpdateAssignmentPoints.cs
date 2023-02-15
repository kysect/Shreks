using ITMO.Dev.ASAP.Application.Dto.Study;
using MediatR;

namespace ITMO.Dev.ASAP.Application.Contracts.Study.Assignments.Commands;

internal static class UpdateAssignmentPoints
{
    public record Command(Guid AssignmentId, double MinPoints, double MaxPoints) : IRequest<Response>;

    public record Response(AssignmentDto Assignment);
}