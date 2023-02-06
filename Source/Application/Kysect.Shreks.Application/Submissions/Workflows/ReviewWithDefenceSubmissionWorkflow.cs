using Kysect.Shreks.Application.Abstractions.Google;
using Kysect.Shreks.Application.Abstractions.Permissions;
using Kysect.Shreks.Application.Dto.Submissions;
using Kysect.Shreks.Common.Resources;
using Kysect.Shreks.DataAccess.Abstractions;

namespace Kysect.Shreks.Application.Submissions.Workflows;

public class ReviewWithDefenceSubmissionWorkflow : SubmissionWorkflowBase
{
    public ReviewWithDefenceSubmissionWorkflow(
        IPermissionValidator permissionValidator,
        IShreksDatabaseContext context,
        ITableUpdateQueue updateQueue) : base(permissionValidator, context, updateQueue) { }

    public override async Task<SubmissionActionMessageDto> SubmissionApprovedAsync(
        Guid issuerId,
        Guid submissionId,
        CancellationToken cancellationToken)
    {
        await PermissionValidator.EnsureSubmissionMentorAsync(issuerId, submissionId, cancellationToken);
        await ExecuteSubmissionCommandAsync(submissionId, cancellationToken, static x => x.MarkAsReviewed());

        string message = UserCommandProcessingMessage.SubmissionMarkAsReviewedAndNeedDefense();
        return new SubmissionActionMessageDto(message);
    }
}