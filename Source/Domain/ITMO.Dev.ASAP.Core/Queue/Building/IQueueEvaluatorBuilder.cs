using ITMO.Dev.ASAP.Core.Queue.Evaluators;
using ITMO.Dev.ASAP.Core.Queue.Filters;

namespace ITMO.Dev.ASAP.Core.Queue.Building;

public interface IQueueEvaluatorBuilder
{
    IQueueEvaluatorBuilder AddEvaluator(ISubmissionEvaluator evaluator);

    SubmissionQueue Build();
}

public interface IQueueFilterBuilder : IQueueEvaluatorBuilder
{
    IQueueFilterBuilder AddFilter(IQueueFilter filter);
}