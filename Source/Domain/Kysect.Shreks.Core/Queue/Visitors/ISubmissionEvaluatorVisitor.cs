using Kysect.Shreks.Core.Queue.Evaluators;
using Kysect.Shreks.Core.Study;

namespace Kysect.Shreks.Core.Queue.Visitors;

public interface ISubmissionEvaluatorVisitor<T>
{
    ValueTask<T> VisitAsync(Submission submission, DeadlineEvaluator evaluator, CancellationToken cancellationToken);
}