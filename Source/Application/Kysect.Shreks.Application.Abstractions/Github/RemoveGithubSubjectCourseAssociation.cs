using Kysect.Shreks.Application.Dto.SubjectCourses;
using MediatR;

namespace Kysect.Shreks.Application.Abstractions.Github;

public static class RemoveGithubSubjectCourseAssociation
{
    public record Command(Guid SubjectCourseId) : IRequest<Response>;

    public record Response(SubjectCourseDto SubjectCourse);
}