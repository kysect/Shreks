using Kysect.Shreks.Core.Queue.Evaluators;
using Kysect.Shreks.Core.Queue.Filters;

namespace Kysect.Shreks.Core.Queue.Building;

public class QueueBuilder : IQueueFilterBuilder
{
    private readonly List<ISubmissionEvaluator> _evaluators;
    private readonly List<IQueueFilter> _filters;

    public QueueBuilder()
    {
        _filters = new List<IQueueFilter>();
        _evaluators = new List<ISubmissionEvaluator>();
    }

    public IQueueFilterBuilder AddFilter(IQueueFilter filter)
    {
        _filters.Add(filter);
        return this;
    }

    public IQueueEvaluatorBuilder AddEvaluator(ISubmissionEvaluator evaluator)
    {
        _evaluators.Add(evaluator);
        return this;
    }

    public SubmissionQueue Build()
    {
        IQueueFilter[] filters = _filters.ToArray();
        ISubmissionEvaluator[] evaluators = _evaluators.ToArray();
        return new SubmissionQueue(filters, evaluators);
    }
}