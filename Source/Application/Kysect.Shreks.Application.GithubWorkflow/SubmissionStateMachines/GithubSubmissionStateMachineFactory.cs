using Kysect.Shreks.Application.Commands.Processors;
using Kysect.Shreks.Application.GithubWorkflow.Abstractions;
using Kysect.Shreks.Application.GithubWorkflow.Abstractions.Models;
using Kysect.Shreks.Application.GithubWorkflow.Extensions;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Core.SubmissionStateWorkflows;
using Kysect.Shreks.DataAccess.Abstractions;
using Microsoft.Extensions.Logging;

namespace Kysect.Shreks.Application.GithubWorkflow.SubmissionStateMachines;

public class GithubSubmissionStateMachineFactory
{
    private readonly IShreksDatabaseContext _context;
    private readonly SubmissionService _shreksCommandProcessor;

    public GithubSubmissionStateMachineFactory(IShreksDatabaseContext context, SubmissionService shreksCommandProcessor)
    {
        _context = context;
        _shreksCommandProcessor = shreksCommandProcessor;
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
                return new ReviewOnlyGithubSubmissionStateMachine(_context, _shreksCommandProcessor, commandProcessor, logger, eventNotifier);

            case SubmissionStateWorkflowType.ReviewWithDefense:
                return new ReviewWithDefenseGithubSubmissionStateMachine(_context, _shreksCommandProcessor, commandProcessor, logger, eventNotifier);

            default:
                throw new ArgumentOutOfRangeException(nameof(subjectCourse.WorkflowType));
        }
    }
}