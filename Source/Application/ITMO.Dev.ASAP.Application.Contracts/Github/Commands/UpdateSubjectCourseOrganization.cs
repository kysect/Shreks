using MediatR;

namespace ITMO.Dev.ASAP.Application.Contracts.Github.Commands;

public static class UpdateSubjectCourseOrganization
{
    public record Command(Guid SubjectCourseId) : IRequest;
}