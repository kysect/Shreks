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
    private readonly IDefaultSubmissionProvider _defaultSubmissionProvider;
    private readonly IMediator _mediator;
    private readonly GithubPullRequestDescriptor _pullRequestDescriptor;

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
        UserDto user = await GetIssuerAsync(cancellationToken);
        return new BaseContext(user.Id, _mediator);
    }

    public async Task<SubmissionContext> SubmissionContextAsync(CancellationToken cancellationToken)
    {
        UserDto user = await GetIssuerAsync(cancellationToken);

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
        UserDto issuer = await GetIssuerAsync(cancellationToken);
        UserDto student = await GetStudentAsync(cancellationToken);

        var query = new GetAssignment.Query(_pullRequestDescriptor.Organization, _pullRequestDescriptor.BranchName);
        GetAssignment.Response response = await _mediator.Send(query, cancellationToken);

        return new UpdateContext(issuer.Id, _mediator, response.Assignment, student, _defaultSubmissionProvider);
    }

    public async Task<PayloadAndAssignmentContext> PayloadAndAssignmentContextAsync(CancellationToken cancellationToken)
    {
        UserDto user = await GetIssuerAsync(cancellationToken);

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
        UserDto user = await GetIssuerAsync(cancellationToken);

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

    private async Task<UserDto> GetIssuerAsync(CancellationToken cancellationToken)
    {
        var query = new GetUserByGithubUsername.Query(_pullRequestDescriptor.Sender);
        GetUserByGithubUsername.Response response = await _mediator.Send(query, cancellationToken);

        return response.User;
    }

    public async Task<UserDto> GetStudentAsync(CancellationToken cancellationToken)
    {
        var query = new GetUserByGithubUsername.Query(_pullRequestDescriptor.Repository);
        GetUserByGithubUsername.Response response = await _mediator.Send(query, cancellationToken);
        
        return response.User;
    }
}