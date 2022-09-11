using Kysect.Shreks.Core.Submissions;

namespace Kysect.Shreks.Application.GithubWorkflow;

public record GithubSubmissionCreationResult(Submission Submission, bool IsCreated);