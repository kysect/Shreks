using Kysect.Shreks.Core.Study;

namespace Kysect.Shreks.Core.SubmissionAssociations;

public partial class GithubPullRequestSubmissionAssociation : SubmissionAssociation
{
    public GithubPullRequestSubmissionAssociation(Submission submission, string organization, 
        string repository, long pullRequestNumber) : base(submission)
    {
        Repository = repository;
        PullRequestNumber = pullRequestNumber;
        Organization = organization;
    }

    public string Organization { get; protected set; }
    public string Repository { get; protected set; }
    public long PullRequestNumber { get; protected set; }
}