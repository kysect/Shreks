using Kysect.Shreks.Application.Abstractions.Permissions;
using Kysect.Shreks.Application.Dto.Submissions;
using Kysect.Shreks.Common.Resources;
using Kysect.Shreks.Core.Submissions;
using Kysect.Shreks.Core.ValueObject;
using Kysect.Shreks.DataAccess.Abstractions;
using MediatR;

namespace Kysect.Shreks.Application.Submissions.Workflows;

public class ReviewOnlySubmissionWorkflow : SubmissionWorkflowBase
{
    public ReviewOnlySubmissionWorkflow(
        IShreksDatabaseContext context,
        IPermissionValidator permissionValidator,
        IPublisher publisher) : base(permissionValidator, context, publisher) { }

    public override async Task<SubmissionActionMessageDto> SubmissionApprovedAsync(
        Guid issuerId,
        Guid submissionId,
        CancellationToken cancellationToken)
    {
        await PermissionValidator.EnsureSubmissionMentorAsync(issuerId, submissionId, cancellationToken);

        Submission submission = await ExecuteSubmissionCommandAsync(
            submissionId,
            cancellationToken,
            static x =>
            {
                if (x.Points is null)
                    x.Rate(Fraction.FromDenormalizedValue(100), 0);
            });

        string message = UserCommandProcessingMessage.ReviewRatedSubmission(submission.Points?.Value ?? 0);
        return new SubmissionActionMessageDto(message);
    }
}