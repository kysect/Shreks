using Kysect.Shreks.Core.SubmissionAssociations;
using Kysect.Shreks.DataAccess.Abstractions;
using Kysect.Shreks.DataAccess.Abstractions.Extensions;
using MediatR;

namespace Kysect.Shreks.Application.Handlers.Github;
using static Abstractions.Github.Commands.AddSubmissionGithubPrAssociation;


public class AddSubmissionGithubPrAssociationHandler : IRequestHandler<Command>
{
    private readonly IShreksDatabaseContext _context;

    public AddSubmissionGithubPrAssociationHandler(IShreksDatabaseContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
    {
        var submission = await _context.Submissions.GetByIdAsync(request.SubmissionId, cancellationToken);

        _context.SubmissionAssociations.Add(new GithubPullRequestSubmissionAssociation(submission, 
                request.Organisation, request.Repository, request.PrNumber));
        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}