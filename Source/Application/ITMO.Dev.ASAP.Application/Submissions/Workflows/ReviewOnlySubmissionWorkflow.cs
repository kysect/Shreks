using ITMO.Dev.ASAP.Application.Abstractions.Permissions;
using ITMO.Dev.ASAP.Application.Dto.Submissions;
using ITMO.Dev.ASAP.Common.Resources;
using ITMO.Dev.ASAP.Core.Submissions;
using ITMO.Dev.ASAP.Core.ValueObject;
using ITMO.Dev.ASAP.DataAccess.Abstractions;
using MediatR;

namespace ITMO.Dev.ASAP.Application.Submissions.Workflows;

public class ReviewOnlySubmissionWorkflow : SubmissionWorkflowBase
{
    public ReviewOnlySubmissionWorkflow(
        IDatabaseContext context,
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