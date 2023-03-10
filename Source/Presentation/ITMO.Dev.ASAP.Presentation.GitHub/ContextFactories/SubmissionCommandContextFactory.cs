using ITMO.Dev.ASAP.Application.Contracts.Github.Commands;
using ITMO.Dev.ASAP.Application.Contracts.Github.Queries;
using ITMO.Dev.ASAP.Application.Dto.Study;
using ITMO.Dev.ASAP.Application.Dto.Users;
using ITMO.Dev.ASAP.Application.GithubWorkflow.Abstractions.Models;
using ITMO.Dev.ASAP.Commands.Contexts;
using ITMO.Dev.ASAP.Commands.Tools;
using MediatR;

namespace ITMO.Dev.ASAP.Presentation.GitHub.ContextFactories;

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

        var query = new GetCurrentUnratedGitHubSubmission.Query(
            _pullRequestDescriptor.Organization,
            _pullRequestDescriptor.Repository,
            _pullRequestDescriptor.PrNumber,
            _pullRequestDescriptor.Payload);

        GetCurrentUnratedGitHubSubmission.Response response = await _mediator.Send(query, cancellationToken);

        return new SubmissionContext(user.Id, _mediator, response.Submission.Id);
    }

    public async Task<UpdateContext> UpdateContextAsync(CancellationToken cancellationToken)
    {
        UserDto issuer = await GetIssuerAsync(cancellationToken);
        UserDto student = await GetStudentAsync(cancellationToken);

        var query = new GetGitHubAssignment.Query(_pullRequestDescriptor.Organization, _pullRequestDescriptor.BranchName);
        GetGitHubAssignment.Response response = await _mediator.Send(query, cancellationToken);

        return new UpdateContext(issuer.Id, _mediator, response.Assignment, student, _defaultSubmissionProvider);
    }

    public async Task<PayloadAndAssignmentContext> PayloadAndAssignmentContextAsync(CancellationToken cancellationToken)
    {
        UserDto user = await GetIssuerAsync(cancellationToken);

        var query = new GetGitHubAssignment.Query(_pullRequestDescriptor.Organization, _pullRequestDescriptor.BranchName);
        GetGitHubAssignment.Response response = await _mediator.Send(query, cancellationToken);

        return new PayloadAndAssignmentContext(
            user.Id,
            _mediator,
            response.Assignment.Id,
            _pullRequestDescriptor.Payload);
    }

    public async Task<CreateSubmissionContext> CreateSubmissionContextAsync(CancellationToken cancellationToken)
    {
        UserDto user = await GetIssuerAsync(cancellationToken);

        var query = new GetGitHubAssignment.Query(_pullRequestDescriptor.Organization, _pullRequestDescriptor.BranchName);
        GetGitHubAssignment.Response response = await _mediator.Send(query, cancellationToken);

        async Task<SubmissionRateDto> CreateFunc(CancellationToken ct)
        {
            var command = new CreateGithubSubmission.Command(
                user.Id,
                response.Assignment.Id,
                _pullRequestDescriptor.Organization,
                _pullRequestDescriptor.Repository,
                _pullRequestDescriptor.PrNumber,
                _pullRequestDescriptor.Payload);

            CreateGithubSubmission.Response commandResponse = await _mediator.Send(command, ct);

            return commandResponse.Submission;
        }

        return new CreateSubmissionContext(user.Id, _mediator, CreateFunc, _pullRequestDescriptor.Payload);
    }

    public async Task<UserDto> GetStudentAsync(CancellationToken cancellationToken)
    {
        var query = new GetGithubUser.Query(_pullRequestDescriptor.Repository);
        GetGithubUser.Response response = await _mediator.Send(query, cancellationToken);

        return response.User;
    }

    private async Task<UserDto> GetIssuerAsync(CancellationToken cancellationToken)
    {
        var query = new GetGithubUser.Query(_pullRequestDescriptor.Sender);
        GetGithubUser.Response response = await _mediator.Send(query, cancellationToken);

        return response.User;
    }
}