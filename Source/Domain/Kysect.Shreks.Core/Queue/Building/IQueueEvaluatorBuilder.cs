using Kysect.Shreks.Core.Queue.Evaluators;
using Kysect.Shreks.Core.Queue.Filters;

namespace Kysect.Shreks.Core.Queue.Building;

public interface IQueueEvaluatorBuilder
{
    IQueueEvaluatorBuilder AddEvaluator(ISubmissionEvaluator evaluator);

    SubmissionQueue Build();
}

public interface IQueueFilterBuilder : IQueueEvaluatorBuilder
{
    IQueueFilterBuilder AddFilter(IQueueFilter filter);
}