using Kysect.Shreks.Core.Study;

namespace Kysect.Shreks.Core.SubmissionAssociations;

public partial class GithubPullRequestSubmissionAssociation : SubmissionAssociation
{
    public GithubPullRequestSubmissionAssociation(Submission submission, int pullRequestNumber)
        : base(submission)
    {
        PullRequestNumber = pullRequestNumber;
    }

    public int PullRequestNumber { get; protected set; }
}