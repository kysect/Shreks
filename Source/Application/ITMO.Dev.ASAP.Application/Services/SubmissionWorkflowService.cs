using ITMO.Dev.ASAP.Application.Abstractions.Permissions;
using ITMO.Dev.ASAP.Application.Abstractions.Submissions;
using ITMO.Dev.ASAP.Application.Submissions.Workflows;
using ITMO.Dev.ASAP.Common.Exceptions;
using ITMO.Dev.ASAP.Core.Study;
using ITMO.Dev.ASAP.Core.Submissions;
using ITMO.Dev.ASAP.Core.SubmissionStateWorkflows;
using ITMO.Dev.ASAP.DataAccess.Abstractions;
using ITMO.Dev.ASAP.DataAccess.Abstractions.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ITMO.Dev.ASAP.Application.Services;

public class SubmissionWorkflowService : ISubmissionWorkflowService
{
    private readonly IDatabaseContext _context;
    private readonly IPermissionValidator _permissionValidator;
    private readonly IPublisher _publisher;

    public SubmissionWorkflowService(
        IDatabaseContext context,
        IPermissionValidator permissionValidator,
        IPublisher publisher)
    {
        _context = context;
        _permissionValidator = permissionValidator;
        _publisher = publisher;
    }

    public async Task<ISubmissionWorkflow> GetSubmissionWorkflowAsync(
        Guid submissionId,
        CancellationToken cancellationToken)
    {
        SubjectCourse? subjectCourse = await _context.Submissions
            .Where(x => x.Id.Equals(submissionId))
            .Select(x => x.GroupAssignment.Assignment.SubjectCourse)
            .SingleOrDefaultAsync(cancellationToken);

        if (subjectCourse is null)
            throw EntityNotFoundException.For<Submission>(submissionId);

        return await GetSubjectCourseWorkflowAsync(subjectCourse.Id, cancellationToken);
    }

    public async Task<ISubmissionWorkflow> GetSubjectCourseWorkflowAsync(
        Guid subjectCourseId,
        CancellationToken cancellationToken)
    {
        SubjectCourse subjectCourse = await _context.SubjectCourses.GetByIdAsync(subjectCourseId, cancellationToken);

        return subjectCourse.WorkflowType switch
        {
            null or SubmissionStateWorkflowType.ReviewOnly
                => new ReviewOnlySubmissionWorkflow(_context, _permissionValidator, _publisher),

            SubmissionStateWorkflowType.ReviewWithDefense
                => new ReviewWithDefenceSubmissionWorkflow(_permissionValidator, _context, _publisher),

            _ => throw new ArgumentOutOfRangeException(
                nameof(subjectCourseId),
                $@"Invalid WorkflowType {subjectCourse.WorkflowType:G}"),
        };
    }
}