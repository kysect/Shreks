using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Core.SubmissionAssociations;
using Kysect.Shreks.Core.Users;

namespace Kysect.Shreks.Core.Submissions;

public partial class GithubSubmission : Submission
{
    public GithubSubmission(
        int code,
        Student student,
        GroupAssignment groupAssignment,
        DateOnly submissionDate,
        string payload,
        string organization,
        string repository,
        long prNumber)
        : base(code, student, groupAssignment, submissionDate, payload)
    {
        var association = new GithubSubmissionAssociation
        (
            this,
            organization,
            repository,
            prNumber
        );

        AddAssociation(association);
    }

    public GithubSubmissionAssociation GetAssociation()
        => Associations.OfType<GithubSubmissionAssociation>().Single();
}