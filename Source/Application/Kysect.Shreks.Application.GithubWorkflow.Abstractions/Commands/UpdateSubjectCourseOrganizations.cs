using MediatR;

namespace Kysect.Shreks.Application.GithubWorkflow.Abstractions.Commands;

public static class UpdateSubjectCourseOrganizations
{
    public record Command() : IRequest<Response>;
    public record Response();
}