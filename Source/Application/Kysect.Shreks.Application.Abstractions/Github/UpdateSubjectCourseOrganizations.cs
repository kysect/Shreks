using MediatR;

namespace Kysect.Shreks.Application.Abstractions.Github;

public static class UpdateSubjectCourseOrganizations
{
    public record Command() : IRequest<Response>;
    public record Response();
}