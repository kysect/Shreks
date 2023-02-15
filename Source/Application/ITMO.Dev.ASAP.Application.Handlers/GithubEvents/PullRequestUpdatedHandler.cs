using ITMO.Dev.ASAP.Application.Abstractions.Submissions;
using ITMO.Dev.ASAP.Application.Abstractions.Submissions.Models;
using ITMO.Dev.ASAP.Application.GithubWorkflow.Factories;
using ITMO.Dev.ASAP.Common.Exceptions;
using ITMO.Dev.ASAP.Core.Study;
using ITMO.Dev.ASAP.DataAccess.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static ITMO.Dev.ASAP.Application.Contracts.GithubEvents.PullRequestUpdated;

namespace ITMO.Dev.ASAP.Application.Handlers.GithubEvents;

internal class PullRequestUpdatedHandler : IRequestHandler<Command, Response>
{
    private readonly IDatabaseContext _context;
    private readonly ISubmissionWorkflowService _submissionWorkflowService;

    public PullRequestUpdatedHandler(
        IDatabaseContext context,
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