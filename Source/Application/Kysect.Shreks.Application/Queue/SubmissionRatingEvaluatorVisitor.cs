using Kysect.Shreks.Application.Abstractions.Submissions.Queries;
using Kysect.Shreks.Core.Queue.Evaluators;
using Kysect.Shreks.Core.Queue.Visitors;
using Kysect.Shreks.Core.Study;
using MediatR;

namespace Kysect.Shreks.Application.Queue;

public class SubmissionRatingEvaluatorVisitor : ISubmissionEvaluatorVisitor<int>
{
    private readonly IMediator _mediator;

    public SubmissionRatingEvaluatorVisitor(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async ValueTask<int> VisitAsync(
        Submission submission,
        DeadlineEvaluator evaluator,
        CancellationToken cancellationToken)
    {
        var deadlineQuery = new GetSubmissionDeadline.Query(submission.Id);
        var deadline = await _mediator.Send(deadlineQuery, cancellationToken);
        
        return deadline.Deadline > submission.SubmissionDateTime ? 1 : 0;
    }
}