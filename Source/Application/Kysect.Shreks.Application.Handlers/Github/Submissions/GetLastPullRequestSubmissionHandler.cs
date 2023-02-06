using Kysect.Shreks.Common.Exceptions;
using Kysect.Shreks.Core.SubmissionAssociations;
using Kysect.Shreks.Core.Submissions;
using Kysect.Shreks.DataAccess.Abstractions;
using Kysect.Shreks.Mapping.Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static Kysect.Shreks.Application.Contracts.Github.Queries.GetLastPullRequestSubmission;

namespace Kysect.Shreks.Application.Handlers.Github.Submissions;

internal class GetLastPullRequestSubmissionHandler : IRequestHandler<Query, Response>
{
    private readonly IShreksDatabaseContext _context;

    public GetLastPullRequestSubmissionHandler(IShreksDatabaseContext context)
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