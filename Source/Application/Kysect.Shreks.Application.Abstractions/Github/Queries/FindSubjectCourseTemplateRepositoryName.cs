using MediatR;

namespace Kysect.Shreks.Application.Abstractions.Github.Queries;

public class FindSubjectCourseTemplateRepositoryName
{
    public record Query(string OrganizationName) : IRequest<Response>;

    public record Response(string? TemplateRepositoryName);
}