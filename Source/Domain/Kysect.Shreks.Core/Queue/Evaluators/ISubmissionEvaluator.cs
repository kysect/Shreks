using Kysect.Shreks.Core.Models;
using Kysect.Shreks.Core.Submissions;

namespace Kysect.Shreks.Core.Queue.Evaluators;

public interface ISubmissionEvaluator
{
    SortingOrder SortingOrder { get; }

    double Evaluate(Submission submission);
}