using ITMO.Dev.ASAP.Common.Exceptions;
using ITMO.Dev.ASAP.Core.SubmissionAssociations;
using ITMO.Dev.ASAP.Core.Submissions;
using ITMO.Dev.ASAP.DataAccess.Abstractions;
using ITMO.Dev.ASAP.Mapping.Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static ITMO.Dev.ASAP.Application.Contracts.Github.Queries.GetCurrentUnratedGitHubSubmission;

namespace ITMO.Dev.ASAP.Application.Handlers.Github.Submissions;

internal class GetCurrentUnratedGitHubSubmissionHandler : IRequestHandler<Query, Response>
{
    private readonly IDatabaseContext _context;

    public GetCurrentUnratedGitHubSubmissionHandler(IDatabaseContext context)
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

        return submission is null
            ? throw EntityNotFoundException.NoUnratedSubmissionInPullRequest(request.Payload)
            : new Response(submission.ToDto());
    }
}