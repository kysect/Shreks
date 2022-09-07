using Kysect.Shreks.Application.Dto.Users;
using MediatR;

namespace Kysect.Shreks.Application.GithubWorkflow.Abstractions.Queries;

public static class GetUserCourseRole
{
    public record Query(Guid SubjectCourseId, Guid UserId) : IRequest<Response>;
    public record Response(UserCourseRole Role);    
}