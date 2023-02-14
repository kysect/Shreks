namespace ITMO.Dev.ASAP.Application.Abstractions.Submissions;

public interface ISubmissionWorkflowService
{
    Task<ISubmissionWorkflow> GetSubmissionWorkflowAsync(Guid submissionId, CancellationToken cancellationToken);

    Task<ISubmissionWorkflow> GetSubjectCourseWorkflowAsync(Guid subjectCourseId, CancellationToken cancellationToken);
}