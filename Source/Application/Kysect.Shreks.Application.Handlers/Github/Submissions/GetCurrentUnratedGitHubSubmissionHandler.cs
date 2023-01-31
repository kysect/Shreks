using Kysect.Shreks.Common.Exceptions;
using Kysect.Shreks.Core.SubmissionAssociations;
using Kysect.Shreks.Core.Submissions;
using Kysect.Shreks.DataAccess.Abstractions;
using Kysect.Shreks.Mapping.Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static Kysect.Shreks.Application.Contracts.Github.Queries.GetCurrentUnratedGitHubSubmission;

namespace Kysect.Shreks.Application.Handlers.Github.Submissions;

internal class GetCurrentUnratedGitHubSubmissionHandler : IRequestHandler<Query, Response>
{
    private readonly IShreksDatabaseContext _context;

    public GetCurrentUnratedGitHubSubmissionHandler(IShreksDatabaseContext context)
    {
        _context = context;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        Submission? submission = await _context.SubmissionAssociations
            .OfType<GithubSubmissionAssociation>()
            .Where(a =>
                a.Organization == request.OrganizationName
                && a.Repository == request.RepositoryName
                && a.PrNumber == request.PullRequestNumber
                && a.Submission.Rating == null)
            .OrderByDescending(a => a.Submission.SubmissionDate)
            .Select(a => a.Submission)
            .FirstOrDefaultAsync(cancellationToken);

        if (submission is null)
            throw EntityNotFoundException.NoUnratedSubmissionInPullRequest(request.Payload);

        return new Response(submission.ToDto());
    }
}