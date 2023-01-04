using Kysect.Shreks.Application.Abstractions.Submissions;
using Kysect.Shreks.Application.Abstractions.Submissions.Models;
using Kysect.Shreks.Application.GithubWorkflow.Factories;
using Kysect.Shreks.Common.Exceptions;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.DataAccess.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static Kysect.Shreks.Application.Contracts.GithubEvents.PullRequestUpdated;

namespace Kysect.Shreks.Application.Handlers.GithubEvents;

internal class PullRequestUpdatedHandler : IRequestHandler<Command, Response>
{
    private readonly IShreksDatabaseContext _context;
    private readonly ISubmissionWorkflowService _submissionWorkflowService;

    public PullRequestUpdatedHandler(
        IShreksDatabaseContext context,
        ISubmissionWorkflowService submissionWorkflowService)
    {
        _context = context;
        _submissionWorkflowService = submissionWorkflowService;
    }

    public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
    {
        SubjectCourse? subjectCourse = await _context.Assignments
            .Where(x => x.Id.Equals(request.AssignmentId))
            .Select(x => x.SubjectCourse)
            .SingleOrDefaultAsync(cancellationToken);

        if (subjectCourse is null)
            throw EntityNotFoundException.For<Assignment>(request.AssignmentId);

        ISubmissionWorkflow workflow = await _submissionWorkflowService.GetSubjectCourseWorkflowAsync(
            subjectCourse.Id, cancellationToken);

        var submissionFactory = new GithubSubmissionFactory(
            _context,
            request.OrganizationName,
            request.RepositoryName,
            request.PullRequestNumber,
            request.Payload);

        SubmissionUpdateResult result = await workflow.SubmissionUpdatedAsync(
            request.IssuerId, request.UserId, request.AssignmentId, submissionFactory, cancellationToken);

        return new Response(result);
    }
}