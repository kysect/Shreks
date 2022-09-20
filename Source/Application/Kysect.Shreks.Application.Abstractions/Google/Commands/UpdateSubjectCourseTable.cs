using MediatR;

namespace Kysect.Shreks.Application.Abstractions.Google.Commands;

public static class UpdateSubjectCourseTable
{
    public record Command(Guid SubjectCourseId) : IRequest<Unit>;
}