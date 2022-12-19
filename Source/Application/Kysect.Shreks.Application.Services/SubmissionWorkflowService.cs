using Kysect.Shreks.Application.Abstractions.Google;
using Kysect.Shreks.Application.Abstractions.Permissions;
using Kysect.Shreks.Application.Abstractions.Submissions;
using Kysect.Shreks.Application.Submissions.Workflows;
using Kysect.Shreks.Common.Exceptions;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Core.Submissions;
using Kysect.Shreks.Core.SubmissionStateWorkflows;
using Kysect.Shreks.DataAccess.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Kysect.Shreks.Application.Services;

public class SubmissionWorkflowService : ISubmissionWorkflowService
{
    private readonly IShreksDatabaseContext _context;
    private readonly IPermissionValidator _permissionValidator;
    private readonly ITableUpdateQueue _tableUpdateQueue;

    public SubmissionWorkflowService(
        IShreksDatabaseContext context,
        IPermissionValidator permissionValidator,
        ITableUpdateQueue tableUpdateQueue)
    {
        _context = context;
        _permissionValidator = permissionValidator;
        _tableUpdateQueue = tableUpdateQueue;
    }

    public async Task<ISubmissionWorkflow> GetWorkflowAsync(Guid submissionId, CancellationToken cancellationToken)
    {
        SubjectCourse? subjectCourse = await _context.Submissions
            .Where(x => x.Id.Equals(submissionId))
            .Select(x => x.GroupAssignment.Assignment.SubjectCourse)
            .SingleOrDefaultAsync(cancellationToken);

        if (subjectCourse is null)
            throw EntityNotFoundException.For<Submission>(submissionId);

        return subjectCourse.WorkflowType switch
        {
            null or SubmissionStateWorkflowType.ReviewOnly
                => new ReviewOnlySubmissionWorkflow(_context, _permissionValidator, _tableUpdateQueue),

            SubmissionStateWorkflowType.ReviewWithDefense
                => new ReviewWithDefenceSubmissionWorkflow(_permissionValidator, _context, _tableUpdateQueue),

            _ => throw new ArgumentOutOfRangeException(
                nameof(subjectCourse.WorkflowType), subjectCourse.WorkflowType, null),
        };
    }
}