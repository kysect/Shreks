using MediatR;

namespace Kysect.Shreks.Application.Abstractions.Github.Queries;

public class GerUserCourseRole
{
    public enum UserCourseRole
    {
        Unknown = 1,
        Student = 2,
        Mentor = 3
    }

    public record Query(Guid SubjectCourseId, Guid UserId) : IRequest<Response>;
    public record Response(UserCourseRole Role);    
}