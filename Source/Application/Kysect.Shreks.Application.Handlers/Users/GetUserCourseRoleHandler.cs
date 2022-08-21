using Kysect.Shreks.Application.Dto.Users;
using Kysect.Shreks.DataAccess.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static Kysect.Shreks.Application.Abstractions.Github.Queries.GerUserCourseRole;

namespace Kysect.Shreks.Application.Handlers.Users;

public class GetUserCourseRoleHandler : IRequestHandler<Query, Response>
{
    private readonly IShreksDatabaseContext _context;

    public GetUserCourseRoleHandler(IShreksDatabaseContext context)
    {
        _context = context;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        var isMentor = await _context
            .SubjectCourses
            .Where(course => course.Id == request.SubjectCourseId)
            .SelectMany(course => course.Mentors)
            .AnyAsync(k => k.User.Id == request.UserId, cancellationToken);

        return new Response(isMentor ? UserCourseRole.Mentor : UserCourseRole.Student);
    }
}