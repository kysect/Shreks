using Kysect.Shreks.Application.Dto.Study;

namespace Kysect.Shreks.WebUI.AdminPanel.Extensions;

public static class WorkflowTypeExtensions
{
    public static string AsString(this SubmissionStateWorkflowTypeDto workflowType)
    {
        return workflowType switch
        {
            SubmissionStateWorkflowTypeDto.ReviewOnly => "Review only",
            SubmissionStateWorkflowTypeDto.ReviewWithDefense => "Review with defense",
            _ => throw new ArgumentOutOfRangeException(nameof(workflowType), workflowType, null),
        };
    }
}