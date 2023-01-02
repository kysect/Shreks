using Kysect.Shreks.Application.Abstractions.Permissions;
using Kysect.Shreks.Application.Abstractions.Submissions;
using Kysect.Shreks.Application.Dto.Submissions;
using MediatR;
using static Kysect.Shreks.Application.Contracts.GithubEvents.PullRequestClosed;

namespace Kysect.Shreks.Application.Handlers.GithubEvents;

internal class PullRequestClosedHandler : IRequestHandler<Command, Response>
{
    private readonly IPermissionValidator _permissionValidator;
    private readonly ISubmissionWorkflowService _submissionWorkflowService;

    public PullRequestClosedHandler(
        ISubmissionWorkflowService submissionWorkflowService,
        IPermissionValidator permissionValidator)
    {
        _submissionWorkflowService = submissionWorkflowService;
        _permissionValidator = permissionValidator;
    }

    public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
    {
        (Guid issuerId, Guid submissionId, bool isMerged) = request;

        ISubmissionWorkflow workflow = await _submissionWorkflowService.GetSubmissionWorkflowAsync(
            submissionId, cancellationToken);

        bool isOrganizationMentor = await _permissionValidator.IsSubmissionMentorAsync(
            issuerId, submissionId, cancellationToken);

#pragma warning disable IDE0072
        SubmissionActionMessageDto message = (isOrganizationMentor, IsMerged: isMerged) switch
        {
            (true, true) => await workflow.SubmissionAcceptedAsync(issuerId, submissionId, cancellationToken),
            (true, false) => await workflow.SubmissionRejectedAsync(issuerId, submissionId, cancellationToken),
            (false, _) => await workflow.SubmissionAbandonedAsync(issuerId, submissionId, isMerged, cancellationToken)
        };
#pragma warning restore IDE0072

        return new Response(message);
    }
}