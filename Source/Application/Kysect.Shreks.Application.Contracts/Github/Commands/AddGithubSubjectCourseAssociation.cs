using Kysect.Shreks.Application.Dto.SubjectCourses;
using MediatR;

namespace Kysect.Shreks.Application.Contracts.Github.Commands;

internal static class AddGithubSubjectCourseAssociation
{
    public record Command(Guid SubjectCourseId, string Organization, string TemplateRepository) : IRequest<Response>;

    public record Response(SubjectCourseDto SubjectCourse);
}