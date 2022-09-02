using Kysect.Shreks.Application.Dto.Study;
using MediatR;

namespace Kysect.Shreks.Application.Abstractions.Study.Commands;

public static class CreateGroupAssignment
{
    public record Command(Guid GroupId, Guid AssignmentId, DateOnly Deadline) : IRequest<Response>;

    public record Response(GroupAssignmentDto GroupAssignment);
}