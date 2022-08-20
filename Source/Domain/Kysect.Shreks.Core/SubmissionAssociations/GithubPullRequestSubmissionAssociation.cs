using Kysect.Shreks.Core.Study;

namespace Kysect.Shreks.Core.SubmissionAssociations;

public partial class GithubPullRequestSubmissionAssociation : SubmissionAssociation
{
    public GithubPullRequestSubmissionAssociation(Submission submission, string organizationName, 
        string repository, int pullRequestNumber) : base(submission)
    {
        PullRequestNumber = pullRequestNumber;
    }

    public string Organization { get; protected set; }
    public string Repository { get; protected set; }
    public int PullRequestNumber { get; protected set; }
}