using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Core.SubmissionAssociations;
using Kysect.Shreks.Core.Tools;
using Kysect.Shreks.Core.Users;

namespace Kysect.Shreks.Core.Submissions;

public partial class GithubSubmission : Submission
{
    public GithubSubmission(
        int code,
        Student student,
        GroupAssignment groupAssignment,
        SpbDateTime submissionDate,
        string payload,
        string organization,
        string repository,
        long prNumber)
        : base(code, student, groupAssignment, submissionDate, payload)
    {
        var association = new GithubSubmissionAssociation(
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