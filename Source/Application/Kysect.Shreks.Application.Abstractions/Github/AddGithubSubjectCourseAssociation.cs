using Kysect.Shreks.Application.Dto.Study;
using MediatR;

namespace Kysect.Shreks.Application.Abstractions.Github;

public static class AddGithubSubjectCourseAssociation
{
    public record Command(Guid SubjectCourseId, string Organization, string TemplateRepository) : IRequest<Response>;

    public record Response(SubjectCourseDto SubjectCourse);
}