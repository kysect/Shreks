using ITMO.Dev.ASAP.Application.Dto.SubjectCourses;
using MediatR;

namespace ITMO.Dev.ASAP.Application.Contracts.Github.Commands;

internal static class RemoveGithubSubjectCourseAssociation
{
    public record Command(Guid SubjectCourseId) : IRequest<Response>;

    public record Response(SubjectCourseDto SubjectCourse);
}