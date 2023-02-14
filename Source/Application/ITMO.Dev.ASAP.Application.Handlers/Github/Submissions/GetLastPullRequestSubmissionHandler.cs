using ITMO.Dev.ASAP.Common.Exceptions;
using ITMO.Dev.ASAP.Core.SubmissionAssociations;
using ITMO.Dev.ASAP.Core.Submissions;
using ITMO.Dev.ASAP.DataAccess.Abstractions;
using ITMO.Dev.ASAP.Mapping.Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static ITMO.Dev.ASAP.Application.Contracts.Github.Queries.GetLastPullRequestSubmission;

namespace ITMO.Dev.ASAP.Application.Handlers.Github.Submissions;

internal class GetLastPullRequestSubmissionHandler : IRequestHandler<Query, Response>
{
    private readonly IDatabaseContext _context;

    public GetLastPullRequestSubmissionHandler(IDatabaseContext context)
    {
        _context = context;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        Submission? submission = await _context.Submissions
            .Where(x => x.Student.UserId.Equals(request.UserId))
            .Where(x => x.GroupAssignment.Assignment.Id.Equals(request.AssignmentId))
            .SelectMany(x => x.Associations)
            .OfType<GithubSubmissionAssociation>()
            .Where(x => x.PrNumber.Equals(request.PullRequestNumber))
            .Select(x => x.Submission)
            .OrderByDescending(x => x.Code)
            .FirstOrDefaultAsync(cancellationToken);

        return submission is null
            ? throw new EntityNotFoundException("Submission not found")
            : new Response(submission.ToDto());
    }
}