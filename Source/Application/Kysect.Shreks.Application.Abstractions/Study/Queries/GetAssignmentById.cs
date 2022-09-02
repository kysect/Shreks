using Kysect.Shreks.Application.Dto.Study;
using MediatR;

namespace Kysect.Shreks.Application.Abstractions.Study.Queries;

public static class GetAssignmentById
{
    public record Query(Guid AssignmentId) : IRequest<Response>;

    public record Response(AssignmentDto Assignment);
}