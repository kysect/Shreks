using ITMO.Dev.ASAP.Application.Dto.SubjectCourses;
using MediatR;

namespace ITMO.Dev.ASAP.Application.Contracts.Github.Commands;

internal static class AddGithubSubjectCourseAssociation
{
    public record Command(
        Guid SubjectCourseId,
        string Organization,
        string TemplateRepository,
        string MentorTeamName) : IRequest<Response>;

    public record Response(SubjectCourseDto SubjectCourse);
}