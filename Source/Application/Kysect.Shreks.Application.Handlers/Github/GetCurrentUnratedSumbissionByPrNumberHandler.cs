using Kysect.Shreks.Core.SubmissionAssociations;
using Kysect.Shreks.DataAccess.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Kysect.Shreks.Application.Handlers.Github;
using static Abstractions.Github.Queries.GetCurrentUnratedSumbissionByPrNumber;


public class GetCurrentUnratedSumbissionByPrNumberHandler : IRequestHandler<Query, Response>
{
    private IShreksDatabaseContext _context;

    public GetCurrentUnratedSumbissionByPrNumberHandler(IShreksDatabaseContext context)
    {
        _context = context;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        var submissionId =  await _context.SubmissionAssociations
            .OfType<GithubPullRequestSubmissionAssociation>()
            .Where(a => a.PullRequestNumber == request.PrNumber
                        && a.Submission.Points.Value != 0.0) //TODO: make points nullable, as 0 is valid rating
            .OrderByDescending(a => a.Submission.SubmissionDate)
            .Select(a => a.Submission.Id)
            .FirstOrDefaultAsync(cancellationToken);

        return new Response(submissionId);
    }
}