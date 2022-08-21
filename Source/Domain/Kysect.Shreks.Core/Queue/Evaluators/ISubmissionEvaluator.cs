using Kysect.Shreks.Core.Models;
using Submission = Kysect.Shreks.Core.Submissions.Submission;

namespace Kysect.Shreks.Core.Queue.Evaluators;

public interface ISubmissionEvaluator
{
    SortingOrder SortingOrder { get; }
    
    double Evaluate(Submission submission);
}