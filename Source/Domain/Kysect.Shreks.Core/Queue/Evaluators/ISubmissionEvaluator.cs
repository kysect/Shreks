using Kysect.Shreks.Core.Models;
using Kysect.Shreks.Core.Queue.Visitors;
using Kysect.Shreks.Core.Study;

namespace Kysect.Shreks.Core.Queue.Evaluators;

public interface ISubmissionEvaluator
{
    SortingOrder SortingOrder { get; }
    
    ValueTask<T> AcceptAsync<T>(
        Submission submission,
        ISubmissionEvaluatorVisitor<T> visitor,
        CancellationToken cancellationToken);
}