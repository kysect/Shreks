using Kysect.Shreks.Core.Models;
using Kysect.Shreks.Core.Queue.Evaluators;
using Kysect.Shreks.Core.Queue.Filters;
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
        IQueryExecutor queryExecutor,
        CancellationToken cancellationToken)
        where TComparable : IComparable<TComparable>
    {
        ArgumentNullException.ThrowIfNull(queryExecutor);

        foreach (var filter in _filters)
        {
            submissionsQuery = filter.Filter(submissionsQuery);
        }

        var submissions = await queryExecutor.ExecuteAsync(submissionsQuery, cancellationToken);

        Submissions = SortedBy(submissions, _orderedEvaluators.Value);
    }

    private IReadOnlyList<SubmissionEvaluator> OrderedEvaluators()
    {
        return _evaluators
            .OrderBy(x => x.Position)
            .ToArray();
    }

    private static IReadOnlyCollection<PositionedSubmission> SortedBy(
        IEnumerable<Submission> submissions,
        IReadOnlyList<ISubmissionEvaluator> evaluators)
    {
        var stepperEvaluators = new ForwardIterator<ISubmissionEvaluator>(evaluators, 0);

        IEnumerable<Submission> sortedSubmissions = SortedBy(
            submissions,
            stepperEvaluators);

        IEnumerable<PositionedSubmission> positionedSubmissions = sortedSubmissions
            .Select((x, i) => new PositionedSubmission(i, x));

        return positionedSubmissions.ToList();
    }

    private static IEnumerable<Submission> SortedBy(
        IEnumerable<Submission> submissions,
        ForwardIterator<ISubmissionEvaluator> evaluators)
    {
        var evaluator = evaluators.Current;

        var groupings = submissions
            .GroupBy(x => evaluator.Evaluate(x));

        var orderedGroupings = evaluator.SortingOrder switch
        {
            SortingOrder.Ascending => groupings.OrderBy(x => x.Key),
            SortingOrder.Descending => groupings.OrderByDescending(x => x.Key),
            _ => throw new ArgumentOutOfRangeException(nameof(evaluator.SortingOrder)),
        };

        if (evaluators.IsAtEnd)
            return orderedGroupings.SelectMany(x => x);

        ForwardIterator<ISubmissionEvaluator> next = evaluators.Next();
        return orderedGroupings.SelectMany(x => SortedBy(x, next));
    }
}