using Kysect.Shreks.Core.Study;
using RichEntity.Annotations;

namespace Kysect.Shreks.Core.Queue;

public partial class PositionedSubmission : IEntity<Guid>
{
    public PositionedSubmission(int position, Submission submission) : this(Guid.NewGuid())
    {
        Position = position;
        Submission = submission;
    }

    public int Position { get; protected init; }

    public virtual Submission Submission { get; protected init; }
}