using ITMO.Dev.ASAP.Application.Abstractions.Submissions;
using ITMO.Dev.ASAP.Application.Dto.Submissions;
using MediatR;
using static ITMO.Dev.ASAP.Application.Contracts.GithubEvents.PullRequestApproved;

namespace ITMO.Dev.ASAP.Application.Handlers.GithubEvents;

internal class PullRequestApprovedHandler : IRequestHandler<Command, Response>
{
    private readonly ISubmissionWorkflowService _submissionWorkflowService;

    public PullRequestApprovedHandler(ISubmissionWorkflowService submissionWorkflowService)
    {
        _submissionWorkflowService = submissionWorkflowService;
    }

    public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
    {
        ISubmissionWorkflow workflow = await _submissionWorkflowService.GetSubmissionWorkflowAsync(
            request.SubmissionId, cancellationToken);

        SubmissionActionMessageDto result = await workflow.SubmissionApprovedAsync(
            request.IssuerId, request.SubmissionId, cancellationToken);

        return new Response(result);
    }
}