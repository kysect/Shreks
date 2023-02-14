using ITMO.Dev.ASAP.Application.Abstractions.Submissions;
using ITMO.Dev.ASAP.Application.Dto.Submissions;
using MediatR;
using static ITMO.Dev.ASAP.Application.Contracts.GithubEvents.PullRequestChangesRequested;

namespace ITMO.Dev.ASAP.Application.Handlers.GithubEvents;

internal class PullRequestChangesRequestedHandler : IRequestHandler<Command, Response>
{
    private readonly ISubmissionWorkflowService _submissionWorkflowService;

    public PullRequestChangesRequestedHandler(ISubmissionWorkflowService submissionWorkflowService)
    {
        _submissionWorkflowService = submissionWorkflowService;
    }

    public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
    {
        ISubmissionWorkflow workflow = await _submissionWorkflowService.GetSubmissionWorkflowAsync(
            request.SubmissionId, cancellationToken);

        SubmissionActionMessageDto result = await workflow.SubmissionNotAcceptedAsync(
            request.IssuerId, request.SubmissionId, cancellationToken);

        return new Response(result);
    }
}