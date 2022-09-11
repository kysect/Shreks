using Kysect.Shreks.Core.Submissions;

namespace Kysect.Shreks.Application.Commands.Contexts;

public class SubmissionContext : BaseContext
{
    public Submission Submission { get; }

    public SubmissionContext(Guid issuerId, Submission submission) 
        : base(issuerId)
    {
        Submission = submission;
    }
}