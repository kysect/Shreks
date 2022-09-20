using MediatR;

namespace Kysect.Shreks.Application.Abstractions.Google.Commands;

public static class UpdateSubjectGroupQueueSheet
{
    public record Command(Guid SubjectCourseId, Guid StudentGroupId) : IRequest<Unit>;
}