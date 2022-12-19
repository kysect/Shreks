using Kysect.Shreks.Application.Abstractions.Submissions;
using Kysect.Shreks.Application.Commands.Processors;
using Kysect.Shreks.Application.GithubWorkflow.Abstractions;
using Kysect.Shreks.Application.GithubWorkflow.Abstractions.Models;
using Kysect.Shreks.Application.GithubWorkflow.Submissions;
using Kysect.Shreks.Application.Validators;
using Kysect.Shreks.DataAccess.Abstractions;
using Microsoft.Extensions.Logging;

namespace Kysect.Shreks.Application.GithubWorkflow.SubmissionStateMachines;

public class GithubSubmissionStateMachineFactory
{
    private readonly GithubSubmissionService _githubSubmissionService;
    private readonly PermissionValidator _permissionValidator;
    private readonly ISubmissionWorkflowService _submissionWorkflowService;

    public GithubSubmissionStateMachineFactory(
        IShreksDatabaseContext context,
        ISubmissionWorkflowService submissionWorkflowService)
    {
        _submissionWorkflowService = submissionWorkflowService;
        _githubSubmissionService = new GithubSubmissionService(context);
        _permissionValidator = new PermissionValidator(context);
    }

    public Task<IGithubSubmissionStateMachine> Create(
        ShreksCommandProcessor commandProcessor,
        GithubPullRequestDescriptor prDescriptor,
        ILogger logger,
        IPullRequestEventNotifier eventNotifier)
    {
        IGithubSubmissionStateMachine adapter = new GithubSubmissionStateMachineAdapter
        (
            _submissionWorkflowService,
            _githubSubmissionService,
            commandProcessor,
            eventNotifier,
            logger,
            _permissionValidator
        );

        return Task.FromResult(adapter);
    }
}