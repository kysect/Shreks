using ITMO.Dev.ASAP.Application.Abstractions.Permissions;
using ITMO.Dev.ASAP.Application.Dto.Submissions;
using ITMO.Dev.ASAP.Common.Resources;
using ITMO.Dev.ASAP.DataAccess.Abstractions;
using MediatR;

namespace ITMO.Dev.ASAP.Application.Submissions.Workflows;

public class ReviewWithDefenceSubmissionWorkflow : SubmissionWorkflowBase
{
    public ReviewWithDefenceSubmissionWorkflow(
        IPermissionValidator permissionValidator,
        IDatabaseContext context,
        IPublisher publisher) : base(permissionValidator, context, publisher) { }

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