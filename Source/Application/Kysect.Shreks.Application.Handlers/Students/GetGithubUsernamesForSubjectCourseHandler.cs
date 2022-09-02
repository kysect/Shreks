using Kysect.Shreks.Core.Extensions;
using Kysect.Shreks.Core.Specifications.Github;
using Kysect.Shreks.DataAccess.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static Kysect.Shreks.Application.Abstractions.Github.Queries.GetGithubUsernamesForSubjectCourse;

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
            .WithSpecification(new GetSubjectCourseGithubUsers(request.SubjectCourseId))
            .Select(association => association.GithubUsername)
            .ToListAsync(cancellationToken: cancellationToken);

        return new Response(githubUsernames);
    }
}