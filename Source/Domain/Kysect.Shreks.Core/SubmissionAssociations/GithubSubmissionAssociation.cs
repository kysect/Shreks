using Kysect.Shreks.Core.Submissions;

namespace Kysect.Shreks.Core.SubmissionAssociations;

public partial class GithubSubmissionAssociation : SubmissionAssociation
{
    public GithubSubmissionAssociation(
        GithubSubmission submission,
        string organization,
        string repository,
        long prNumber)
        : base(submission)
    {
        Repository = repository;
        PrNumber = prNumber;
        Organization = organization;
    }

    public string Organization { get; protected set; }

    public string Repository { get; protected set; }

    public long PrNumber { get; protected set; }
}