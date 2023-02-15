using MediatR;

namespace ITMO.Dev.ASAP.DeveloperEnvironment.Requests;

public static class DeleteSubject
{
    public record Command(Guid SubjectId) : IRequest;
}