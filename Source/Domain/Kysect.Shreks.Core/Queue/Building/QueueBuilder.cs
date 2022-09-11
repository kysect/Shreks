using Kysect.Shreks.Core.Queue.Evaluators;
using Kysect.Shreks.Core.Queue.Filters;

namespace Kysect.Shreks.Core.Queue.Building;

public class QueueBuilder : IQueueFilterBuilder
{
    private readonly List<IQueueFilter> _filters;

    private readonly List<ISubmissionEvaluator> _evaluators;

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
        SubmissionQueueFilter[] filters = _filters.OfType<SubmissionQueueFilter>().ToArray();
        SubmissionEvaluator[] evaluators = _evaluators.OfType<SubmissionEvaluator>().ToArray();
        return new SubmissionQueue(filters, evaluators);
    }
}