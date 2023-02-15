using ITMO.Dev.ASAP.Application.Dto.Study;
using MediatR;

namespace ITMO.Dev.ASAP.Application.Contracts.Study.Assignments.Queries;

internal static class GetAssignmentById
{
    public record Query(Guid AssignmentId) : IRequest<Response>;

    public record Response(AssignmentDto Assignment);
}