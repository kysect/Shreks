using MediatR;

namespace Kysect.Shreks.Application.Contracts.Github.Commands;

public static class UpdateSubjectCourseOrganizations
{
    public record Command() : IRequest<Response>;
    public record Response();
}