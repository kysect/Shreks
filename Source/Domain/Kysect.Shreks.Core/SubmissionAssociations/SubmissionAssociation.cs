using Kysect.Shreks.Core.Study;
using RichEntity.Annotations;

namespace Kysect.Shreks.Core.SubmissionAssociations;

public abstract partial class SubmissionAssociation : IEntity<Guid>
{
    protected SubmissionAssociation(Submission submission) : this(Guid.NewGuid())
    {
        Submission = submission;
    }

    public virtual Submission Submission { get; protected init; }
}