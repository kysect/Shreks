using Kysect.Shreks.Application.Commands.Processors;
using Kysect.Shreks.Application.GithubWorkflow.Abstractions;
using Kysect.Shreks.Application.GithubWorkflow.Abstractions.Models;
using Kysect.Shreks.Application.GithubWorkflow.Extensions;
using Kysect.Shreks.Application.GithubWorkflow.Submissions;
using Kysect.Shreks.Application.Validators;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Core.SubmissionStateWorkflows;
using Kysect.Shreks.DataAccess.Abstractions;
using Microsoft.Extensions.Logging;

namespace Kysect.Shreks.Application.GithubWorkflow.SubmissionStateMachines;

public class GithubSubmissionStateMachineFactory
{
    private readonly IShreksDatabaseContext _context;
    private readonly SubmissionService _shreksCommandProcessor;
    private readonly GithubSubmissionService _githubSubmissionService;
    private readonly PermissionValidator _permissionValidator;

    public GithubSubmissionStateMachineFactory(IShreksDatabaseContext context, SubmissionService shreksCommandProcessor)
    {
        _context = context;
        _shreksCommandProcessor = shreksCommandProcessor;
        _githubSubmissionService = new GithubSubmissionService(_context);
        _permissionValidator = new PermissionValidator(_context);
    }

    public async Task<IGithubSubmissionStateMachine> Create(
        ShreksCommandProcessor commandProcessor,
        GithubPullRequestDescriptor prDescriptor,
        ILogger logger,
        IPullRequestEventNotifier eventNotifier)
    {
        SubjectCourse subjectCourse = await _context.SubjectCourseAssociations.GetSubjectCourseByOrganization(prDescriptor.Organization, CancellationToken.None);

        switch (subjectCourse.WorkflowType)
        {
            case null:
            case SubmissionStateWorkflowType.ReviewOnly:
                return new ReviewOnlyGithubSubmissionStateMachine(_shreksCommandProcessor, commandProcessor, _githubSubmissionService, logger, eventNotifier);

            case SubmissionStateWorkflowType.ReviewWithDefense:
                return new ReviewWithDefenseGithubSubmissionStateMachine(_shreksCommandProcessor, commandProcessor, logger, eventNotifier, _githubSubmissionService, _permissionValidator);

            default:
                throw new ArgumentOutOfRangeException(nameof(subjectCourse.WorkflowType));
        }
    }
}