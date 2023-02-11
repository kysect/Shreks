using ITMO.Dev.ASAP.Core.Models;
using ITMO.Dev.ASAP.Core.Submissions;

namespace ITMO.Dev.ASAP.Core.Queue.Evaluators;

public interface ISubmissionEvaluator
{
    SortingOrder SortingOrder { get; }

    double Evaluate(Submission submission);
}