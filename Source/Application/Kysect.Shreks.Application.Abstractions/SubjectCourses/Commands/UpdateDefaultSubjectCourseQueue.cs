using MediatR;

namespace Kysect.Shreks.Application.Abstractions.SubjectCourses.Commands;

public static class UpdateDefaultSubjectCourseQueue
{
    public record struct Command(Guid SubjectCourseId) : IRequest;
}