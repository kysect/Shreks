using Kysect.Shreks.Core.Extensions;
using Kysect.Shreks.Core.Models;
using Kysect.Shreks.Core.Queue.Evaluators;
using Kysect.Shreks.Core.Queue.Filters;
using Kysect.Shreks.Core.Queue.Visitors;
using Kysect.Shreks.Core.Study;
using RichEntity.Annotations;

namespace Kysect.Shreks.Core.Queue;

public partial class SubmissionQueue : IEntity<Guid>
{
    private readonly IReadOnlyCollection<QueueFilter> _filters;
    private readonly IReadOnlyCollection<SubmissionEvaluator> _evaluators;
    private readonly Lazy<IReadOnlyList<SubmissionEvaluator>> _orderedEvaluators;

    public SubmissionQueue(
        IReadOnlyCollection<QueueFilter> filters,
        IReadOnlyList<SubmissionEvaluator> evaluators)
        : this(Guid.NewGuid())
    {
        ArgumentNullException.ThrowIfNull(filters);
        ArgumentNullException.ThrowIfNull(evaluators);

        _filters = filters;
        _evaluators = evaluators;
        Submissions = Array.Empty<PositionedSubmission>();
        _orderedEvaluators = new Lazy<IReadOnlyList<SubmissionEvaluator>>(OrderedEvaluators);
    }

    public virtual IReadOnlyCollection<PositionedSubmission> Submissions { get; protected set; }

    public virtual IReadOnlyCollection<QueueFilter> Filters => _filters;
    public virtual IReadOnlyCollection<SubmissionEvaluator> Evaluators => _evaluators;

    public async Task UpdateSubmissions<TComparable>(
        IQueryable<Submission> submissionsQuery,
        IQueueFilterVisitor<IQueryable<Submission>> filterVisitor,
        ISubmissionEvaluatorVisitor<TComparable> evaluatorVisitor,
        IQueryExecutor queryExecutor,
        CancellationToken cancellationToken)
        where TComparable : IComparable<TComparable>
    {
        ArgumentNullException.ThrowIfNull(queryExecutor);

        foreach (var filter in _filters)
        {
            submissionsQuery = await filter.AcceptAsync(submissionsQuery, filterVisitor, cancellationToken);
        }

        var submissions = await queryExecutor.ExecuteAsync(submissionsQuery, cancellationToken);

        Submissions = await SortedBy(
            submissions,
            _orderedEvaluators.Value,
            evaluatorVisitor,
            cancellationToken);
    }

    private IReadOnlyList<SubmissionEvaluator> OrderedEvaluators()
    {
        return _evaluators
            .OrderBy(x => x.Position)
            .ToArray();
    }

    private static async Task<IReadOnlyCollection<PositionedSubmission>> SortedBy<TComparable>(
        IEnumerable<Submission> submissions,
        IReadOnlyList<ISubmissionEvaluator> evaluators,
        ISubmissionEvaluatorVisitor<TComparable> visitor,
        CancellationToken cancellationToken)
        where TComparable : IComparable<TComparable>
    {
        var stepperEvaluators = new ForwardIterator<ISubmissionEvaluator>(evaluators, 0);

        IAsyncEnumerable<Submission> sortedSubmissions = SortedBy(
            submissions.AsAsyncEnumerable(cancellationToken),
            stepperEvaluators,
            visitor,
            cancellationToken);

        IAsyncEnumerable<PositionedSubmission> positionedSubmissions = sortedSubmissions
            .Select((x, i) => new PositionedSubmission(i, x));

        return await positionedSubmissions.ToListAsync(cancellationToken);
    }

    private static IAsyncEnumerable<Submission> SortedBy<TComparable>(
        IAsyncEnumerable<Submission> submissions,
        ForwardIterator<ISubmissionEvaluator> evaluators,
        ISubmissionEvaluatorVisitor<TComparable> visitor,
        CancellationToken cancellationToken)
        where TComparable : IComparable<TComparable>
    {
        var evaluator = evaluators.Current;

        var groupings = submissions
            .GroupByAwait(x => evaluator.AcceptAsync(x, visitor, cancellationToken));

        var orderedGroupings = evaluator.SortingOrder switch
        {
            SortingOrder.Ascending => groupings.OrderBy(x => x.Key),
            SortingOrder.Descending => groupings.OrderByDescending(x => x.Key),
            _ => throw new ArgumentOutOfRangeException(nameof(evaluator.SortingOrder)),
        };

        if (evaluators.IsAtEnd)
            return orderedGroupings.SelectMany(x => x);

        ForwardIterator<ISubmissionEvaluator> next = evaluators.Next();
        return orderedGroupings.SelectMany(x => SortedBy(x, next, visitor, cancellationToken));
    }
}