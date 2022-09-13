using Kysect.Shreks.Application.Dto.Github;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Core.Submissions;

namespace Kysect.Shreks.Application.GithubWorkflow.Submissions;

public interface IGithubSubmissionService
{
    Task<Submission> GetLastSubmissionByPr(GithubPullRequestDescriptor pullRequestDescriptor);
    Task<Assignment> GetAssignmentByBranchAndSubjectCourse(Guid subjectCourseId, GithubPullRequestDescriptor pullRequestDescriptor, CancellationToken cancellationToken);
    Task<Submission> GetCurrentUnratedSubmissionByPrNumber(GithubPullRequestDescriptor pullRequestDescriptor, CancellationToken cancellationToken);
}