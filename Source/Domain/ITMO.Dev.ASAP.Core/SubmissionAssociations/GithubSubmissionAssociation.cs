using ITMO.Dev.ASAP.Core.Submissions;

namespace ITMO.Dev.ASAP.Core.SubmissionAssociations;

public partial class GithubSubmissionAssociation : SubmissionAssociation
{
    public GithubSubmissionAssociation(
        Guid id,
        GithubSubmission submission,
        string organization,
        string repository,
        long prNumber)
        : base(id, submission)
    {
        Repository = repository;
        PrNumber = prNumber;
        Organization = organization;
    }

    public string Organization { get; protected set; }

    public string Repository { get; protected set; }

    public long PrNumber { get; protected set; }
}