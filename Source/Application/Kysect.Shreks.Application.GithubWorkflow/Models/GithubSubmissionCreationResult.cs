using Kysect.Shreks.Core.Submissions;

namespace Kysect.Shreks.Application.GithubWorkflow.Models;

public record GithubSubmissionCreationResult(Submission Submission, bool IsCreated);