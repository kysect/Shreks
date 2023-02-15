using ITMO.Dev.ASAP.Core.Submissions;
using RichEntity.Annotations;

namespace ITMO.Dev.ASAP.Core.SubmissionAssociations;

public abstract partial class SubmissionAssociation : IEntity<Guid>
{
    protected SubmissionAssociation(Guid id, Submission submission) : this(id)
    {
        Submission = submission;
    }

    public virtual Submission Submission { get; protected init; }
}