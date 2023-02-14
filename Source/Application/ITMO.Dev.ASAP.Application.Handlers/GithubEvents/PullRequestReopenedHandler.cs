using ITMO.Dev.ASAP.Application.Abstractions.Submissions;
using ITMO.Dev.ASAP.Application.Dto.Submissions;
using MediatR;
using static ITMO.Dev.ASAP.Application.Contracts.GithubEvents.PullRequestReopened;

namespace ITMO.Dev.ASAP.Application.Handlers.GithubEvents;

internal class PullRequestReopenedHandler : IRequestHandler<Command, Response>
{
    private readonly ISubmissionWorkflowService _submissionWorkflowService;

    public PullRequestReopenedHandler(ISubmissionWorkflowService submissionWorkflowService)
    {
        _submissionWorkflowService = submissionWorkflowService;
    }

    public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
    {
        ISubmissionWorkflow workflow = await _submissionWorkflowService.GetSubmissionWorkflowAsync(
            request.SubmissionId, cancellationToken);

        SubmissionActionMessageDto message = await workflow.SubmissionReactivatedAsync(
            request.IssuerId, request.SubmissionId, cancellationToken);

        return new Response(message);
    }
}