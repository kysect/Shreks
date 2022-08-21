using RichEntity.Annotations;
using Submission = Kysect.Shreks.Core.Submissions.Submission;

namespace Kysect.Shreks.Core.SubmissionAssociations;

public abstract partial class SubmissionAssociation : IEntity<Guid>
{
    protected SubmissionAssociation(Submission submission) : this(Guid.NewGuid())
    {
        Submission = submission;
    }

    public virtual Submission Submission { get; protected init; }
}