using MediatR;

namespace ITMO.Dev.ASAP.Application.Contracts.Github.Commands;

internal static class UpdateSubjectCourseOrganizations
{
    public record Command : IRequest;
}