using Kysect.Shreks.Application.Dto.Study;
using MediatR;

namespace Kysect.Shreks.Application.Contracts.Study.Commands;

internal static class CreateAssignment
{
    public record Command(
        Guid SubjectCourseId,
        string Title,
        string ShortName,
        int Order,
        double MinPoints,
        double MaxPoints) : IRequest<Response>;

    public record Response(AssignmentDto Assignment);
}