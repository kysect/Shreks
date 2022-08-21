using Kysect.Shreks.Core.Submissions;

namespace Kysect.Shreks.Core.SubmissionAssociations;

public partial class GithubSubmissionAssociation : SubmissionAssociation
{
    public GithubSubmissionAssociation(
        GithubSubmission submission,
        string organization,
        string repository,
        long pullRequestNumber) : base(submission)
    {
        Repository = repository;
        PullRequestNumber = pullRequestNumber;
        Organization = organization;
    }

    public string Organization { get; protected set; }
    public string Repository { get; protected set; }
    public long PullRequestNumber { get; protected set; }
}