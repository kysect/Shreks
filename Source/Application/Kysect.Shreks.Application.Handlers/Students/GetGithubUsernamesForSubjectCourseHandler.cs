using Kysect.Shreks.Core.UserAssociations;
using Kysect.Shreks.DataAccess.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static Kysect.Shreks.Application.Abstractions.Students.GetGithubUsernamesForSubjectCourse;

namespace Kysect.Shreks.Application.Handlers.Students;

public class GetGithubUsernamesForSubjectCourseHandler : IRequestHandler<Query, Response>
{

    private readonly IShreksDatabaseContext _context;

    public GetGithubUsernamesForSubjectCourseHandler(IShreksDatabaseContext context)
    {
        _context = context;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        List<string> githubUsernames = await _context
            .SubjectCourseGroups
            .Where(subjectCourseGroup => subjectCourseGroup.SubjectCourseId == request.SubjectCourseId)
            .Select(group => group.StudentGroup)
            .SelectMany(group => group.Students)
            .SelectMany(student => student.User.Associations)
            .OfType<GithubUserAssociation>()
            .Select(association => association.GithubUsername)
            .ToListAsync(cancellationToken: cancellationToken);

        return new Response(githubUsernames);
    }
}