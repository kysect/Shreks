using Kysect.Shreks.Application.Contracts.Github.Commands;
using Kysect.Shreks.Application.Contracts.Github.Queries;
using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Application.Dto.Users;
using Kysect.Shreks.Application.GithubWorkflow.Abstractions.Models;
using Kysect.Shreks.Commands.Contexts;
using Kysect.Shreks.Commands.Tools;
using MediatR;

namespace Kysect.Shreks.Presentation.GitHub.ContextFactories;

public class SubmissionCommandContextFactory : ISubmissionCommandContextFactory
{
    private readonly GithubPullRequestDescriptor _pullRequestDescriptor;
    private readonly IMediator _mediator;
    private readonly IDefaultSubmissionProvider _defaultSubmissionProvider;

    public SubmissionCommandContextFactory(
        GithubPullRequestDescriptor pullRequestDescriptor,
        IMediator mediator,
        IDefaultSubmissionProvider defaultSubmissionProvider)
    {
        _pullRequestDescriptor = pullRequestDescriptor;
        _mediator = mediator;
        _defaultSubmissionProvider = defaultSubmissionProvider;
    }

    public async Task<BaseContext> BaseContextAsync(CancellationToken cancellationToken)
    {
        UserDto user = await GetUserAsync(cancellationToken);
        return new BaseContext(user.Id, _mediator);
    }

    public async Task<SubmissionContext> SubmissionContextAsync(CancellationToken cancellationToken)
    {
        UserDto user = await GetUserAsync(cancellationToken);

        var query = new GetCurrentUnratedSubmission.Query(
            _pullRequestDescriptor.Organization,
            _pullRequestDescriptor.Repository,
            _pullRequestDescriptor.PrNumber,
            _pullRequestDescriptor.Payload);

        GetCurrentUnratedSubmission.Response response = await _mediator.Send(query, cancellationToken);

        return new SubmissionContext(user.Id, _mediator, response.Submission.Id);
    }

    public async Task<UpdateContext> UpdateContextAsync(CancellationToken cancellationToken)
    {
        UserDto user = await GetUserAsync(cancellationToken);

        var query = new GetAssignment.Query(_pullRequestDescriptor.Organization, _pullRequestDescriptor.BranchName);
        GetAssignment.Response response = await _mediator.Send(query, cancellationToken);

        return new UpdateContext(user.Id, _mediator, response.Assignment, user, _defaultSubmissionProvider);
    }

    public async Task<PayloadAndAssignmentContext> PayloadAndAssignmentContextAsync(CancellationToken cancellationToken)
    {
        UserDto user = await GetUserAsync(cancellationToken);

        var query = new GetAssignment.Query(_pullRequestDescriptor.Organization, _pullRequestDescriptor.BranchName);
        GetAssignment.Response response = await _mediator.Send(query, cancellationToken);

        return new PayloadAndAssignmentContext
        (
            user.Id,
            _mediator,
            response.Assignment.Id,
            _pullRequestDescriptor.Payload
        );
    }

    public async Task<CreateSubmissionContext> CreateSubmissionContextAsync(CancellationToken cancellationToken)
    {
        UserDto user = await GetUserAsync(cancellationToken);

        var query = new GetAssignment.Query(_pullRequestDescriptor.Organization, _pullRequestDescriptor.BranchName);
        GetAssignment.Response response = await _mediator.Send(query, cancellationToken);

        async Task<SubmissionRateDto> CreateFunc(CancellationToken ct)
        {
            var command = new CreateGithubSubmission.Command
            (
                user.Id,
                response.Assignment.Id,
                _pullRequestDescriptor.Organization,
                _pullRequestDescriptor.Repository,
                _pullRequestDescriptor.PrNumber,
                _pullRequestDescriptor.Payload
            );

            CreateGithubSubmission.Response commandResponse = await _mediator.Send(command, ct);

            return commandResponse.Submission;
        }

        return new CreateSubmissionContext(user.Id, _mediator, CreateFunc, _pullRequestDescriptor.Payload);
    }

    private async Task<UserDto> GetUserAsync(CancellationToken cancellationToken)
    {
        var query = new GetUserByGithubUsername.Query(_pullRequestDescriptor.Sender);
        GetUserByGithubUsername.Response response = await _mediator.Send(query, cancellationToken);

        return response.User;
    }
}