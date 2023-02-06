using Kysect.Shreks.Application.Dto.Study;
using MediatR;

namespace Kysect.Shreks.Application.Contracts.Study.Assignments.Queries;

internal static class GetAssignmentById
{
    public record Query(Guid AssignmentId) : IRequest<Response>;

    public record Response(AssignmentDto Assignment);
}