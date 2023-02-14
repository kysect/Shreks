using ITMO.Dev.ASAP.Core.Study;
using ITMO.Dev.ASAP.Core.SubmissionAssociations;
using ITMO.Dev.ASAP.Core.Tools;
using ITMO.Dev.ASAP.Core.Users;

namespace ITMO.Dev.ASAP.Core.Submissions;

public partial class GithubSubmission : Submission
{
    public GithubSubmission(
        Guid id,
        int code,
        Student student,
        GroupAssignment groupAssignment,
        SpbDateTime submissionDate,
        string payload,
        string organization,
        string repository,
        long prNumber)
        : base(id, code, student, groupAssignment, submissionDate, payload)
    {
        var association = new GithubSubmissionAssociation(
            Guid.NewGuid(),
            this,
            organization,
            repository,
            prNumber);

        AddAssociation(association);
    }

    public GithubSubmissionAssociation GetAssociation()
    {
        return Associations.OfType<GithubSubmissionAssociation>().Single();
    }
}