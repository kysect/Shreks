using Kysect.Shreks.Common.Exceptions;
using Kysect.Shreks.Core.Models;
using Kysect.Shreks.Core.Queue.Evaluators;
using Kysect.Shreks.Core.Queue.Filters;
using Kysect.Shreks.Core.Submissions;
using RichEntity.Annotations;

namespace Kysect.Shreks.Core.Queue;

public partial class SubmissionQueue : IEntity<Guid>
{
    private readonly IReadOnlyCollection<IQueueFilter> _filters;
    private readonly IReadOnlyList<ISubmissionEvaluator> _evaluators;

    public SubmissionQueue(
        IReadOnlyCollection<IQueueFilter> filters,
        IReadOnlyList<ISubmissionEvaluator> evaluators)
        : this(Guid.NewGuid())
    {
        ArgumentNullException.ThrowIfNull(filters);
        ArgumentNullException.ThrowIfNull(evaluators);

        _filters = filters;
        _evaluators = evaluators;
    }

    public async Task<IEnumerable<Submission>> UpdateSubmissions(
        IQueryable<Submission> query,
        IQueryExecutor queryExecutor,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(query);
        ArgumentNullException.ThrowIfNull(queryExecutor);

        query = _filters.Aggregate(query, (current, filter) => filter.Filter(current));
        IReadOnlyCollection<Submission> submissions = await queryExecutor.ExecuteAsync(query, cancellationToken);

        return SortedBy(submissions, _evaluators);
    }

    private static IEnumerable<Submission> SortedBy(
        IEnumerable<Submission> submissions,
        IReadOnlyList<ISubmissionEvaluator> evaluators)
    {
        var stepperEvaluators = new ForwardIterator<ISubmissionEvaluator>(evaluators, 0);
        return SortedBy(submissions, stepperEvaluators);
    }

    private static IEnumerable<Submission> SortedBy(
        IEnumerable<Submission> submissions,
        ForwardIterator<ISubmissionEvaluator> evaluators)
    {
        var evaluator = evaluators.Current;

        IEnumerable<IGrouping<double, Submission>> groupings = submissions
            .GroupBy(x => evaluator.Evaluate(x));

        IOrderedEnumerable<IGrouping<double, Submission>> orderedGroupings = evaluator.SortingOrder switch
        {
            SortingOrder.Ascending => groupings.OrderBy(x => x.Key),
            SortingOrder.Descending => groupings.OrderByDescending(x => x.Key),
            _ => throw new UnsupportedOperationException(nameof(evaluator.SortingOrder)),
        };

        if (evaluators.IsAtEnd)
            return orderedGroupings.SelectMany(x => x);

        ForwardIterator<ISubmissionEvaluator> next = evaluators.Next();
        return orderedGroupings.SelectMany(x => SortedBy(x, next));
    }
}