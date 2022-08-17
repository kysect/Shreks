using Kysect.Shreks.Core.Extensions;
using Kysect.Shreks.Core.Models;
using Kysect.Shreks.Core.Queue.Evaluators;
using Kysect.Shreks.Core.Queue.Visitors;
using Kysect.Shreks.Core.Study;
using RichEntity.Annotations;

namespace Kysect.Shreks.Core.Queue;

public partial class SubmissionQueue : IEntity<Guid>
{
    public SubmissionQueue(QueueConfiguration configuration) : this(Guid.NewGuid())
    {
        ArgumentNullException.ThrowIfNull(configuration);

        Submissions = Array.Empty<Submission>();
        Configuration = configuration;
    }

    public QueueConfiguration Configuration { get; protected init; }

    public virtual IReadOnlyCollection<Submission> Submissions { get; protected set; }

    public async Task UpdateSubmissions(
        IQueryable<Submission> submissionsQuery,
        IQueueFilterVisitor<IQueryable<Submission>> filterVisitor,
        ISubmissionEvaluatorVisitor<int> evaluatorVisitor,
        IQueryExecutor queryExecutor,
        CancellationToken cancellationToken)
    {
        foreach (var filter in Configuration.Filters)
        {
            submissionsQuery = await filter.AcceptAsync(submissionsQuery, filterVisitor, cancellationToken);
        }

        var submissions = await queryExecutor.ExecuteAsync(submissionsQuery, cancellationToken);

        Submissions = await SortedBy(
            submissions,
            Configuration.Evaluators,
            evaluatorVisitor,
            cancellationToken);
    }

    private static async Task<IReadOnlyCollection<Submission>> SortedBy(
        IEnumerable<Submission> submissions,
        IReadOnlyList<ISubmissionEvaluator> evaluators,
        ISubmissionEvaluatorVisitor<int> visitor,
        CancellationToken cancellationToken)
    {
        var stepperEvaluators = new StepperCollection<ISubmissionEvaluator>(evaluators, 0);

        IAsyncEnumerable<Submission> sortedSubmissions = SortedBy(
            submissions.AsAsyncEnumerable(cancellationToken),
            stepperEvaluators,
            visitor,
            cancellationToken);

        return await sortedSubmissions.ToListAsync(cancellationToken);
    }

    private static IAsyncEnumerable<Submission> SortedBy(
        IAsyncEnumerable<Submission> submissions,
        StepperCollection<ISubmissionEvaluator> evaluators,
        ISubmissionEvaluatorVisitor<int> visitor,
        CancellationToken cancellationToken)
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

        if (evaluators.AtEnd)
            return orderedGroupings.SelectMany(x => x);

        StepperCollection<ISubmissionEvaluator> next = evaluators.Next();
        return orderedGroupings.SelectMany(x => SortedBy(x, next, visitor, cancellationToken));
    }
}