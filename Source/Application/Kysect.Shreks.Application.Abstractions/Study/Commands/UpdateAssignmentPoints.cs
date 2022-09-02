using Kysect.Shreks.Application.Dto.Study;
using MediatR;

namespace Kysect.Shreks.Application.Abstractions.Study.Commands;

public static class UpdateAssignmentPoints
{
    public record Command(Guid AssignmentId, double MinPoints, double MaxPoints) : IRequest<Response>;

    public record Response(AssignmentDto Assignment);
}