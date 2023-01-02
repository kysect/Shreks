using Kysect.Shreks.Application.Abstractions.Submissions;
using Kysect.Shreks.Application.Dto.Submissions;
using MediatR;
using static Kysect.Shreks.Application.Contracts.GithubEvents.PullRequestApproved;

namespace Kysect.Shreks.Application.Handlers.GithubEvents;

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