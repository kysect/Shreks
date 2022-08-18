using Kysect.Shreks.Core.Queue.Visitors;
using Kysect.Shreks.Core.Study;

namespace Kysect.Shreks.Core.Queue.Filters
{
    public partial class GroupQueueFilter : QueueFilter
    {
        protected GroupQueueFilter(IReadOnlyCollection<StudentGroup> groups)
        {
            Groups = groups;
        }

        public virtual IReadOnlyCollection<StudentGroup> Groups { get; protected init; }

        public override ValueTask<T> AcceptAsync<T>(
            T value,
            IQueueFilterVisitor<T> visitor,
            CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(visitor);
            return visitor.VisitAsync(value, this, cancellationToken);
        }
    }
}