using Kysect.Shreks.Application.Dto.Study;
using MediatR;

namespace Kysect.Shreks.Application.Abstractions.Github.Commands;

public static class RemoveGithubSubjectCourseAssociation
{
    public record Command(Guid SubjectCourseId) : IRequest<Response>;

    public record Response(SubjectCourseDto SubjectCourse);
}