namespace Kysect.Shreks.Application.Abstractions.Submissions;

public interface ISubmissionWorkflowService
{
    Task<ISubmissionWorkflow> GetWorkflowAsync(Guid submissionId, CancellationToken cancellationToken);
}