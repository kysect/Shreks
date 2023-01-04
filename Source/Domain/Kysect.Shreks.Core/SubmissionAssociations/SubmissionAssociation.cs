using Kysect.Shreks.Core.Submissions;
using RichEntity.Annotations;

namespace Kysect.Shreks.Core.SubmissionAssociations;

public abstract partial class SubmissionAssociation : IEntity<Guid>
{
    protected SubmissionAssociation(Guid id, Submission submission) : this(id)
    {
        Submission = submission;
    }

    public virtual Submission Submission { get; protected init; }
}