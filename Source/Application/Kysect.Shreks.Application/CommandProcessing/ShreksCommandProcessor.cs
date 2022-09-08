using Kysect.Shreks.Application.Abstractions.Google;
using Kysect.Shreks.Application.Extensions;
using Kysect.Shreks.Application.Validators;
using Kysect.Shreks.Core.Models;
using Kysect.Shreks.Core.Submissions;
using Kysect.Shreks.DataAccess.Abstractions;
using Kysect.Shreks.DataAccess.Abstractions.Extensions;

namespace Kysect.Shreks.Application.CommandProcessing;

public class ShreksCommandProcessor
{
    private readonly IShreksDatabaseContext _context;
    private readonly ITableUpdateQueue _updateQueue;

    public ShreksCommandProcessor(IShreksDatabaseContext context, ITableUpdateQueue updateQueue)
    {
        _context = context;
        _updateQueue = updateQueue;
    }

    public async Task<Submission> UpdateSubmission(Guid submissionId, Guid userId, SubmissionState state, CancellationToken cancellationToken)
    {
        Submission submission = await _context.Submissions.GetByIdAsync(submissionId, cancellationToken);

        PermissionValidator.EnsureHasAccessToRepository(userId, submission);

        submission.State = state;
        await _context.SaveChangesAsync(cancellationToken);

        _updateQueue.EnqueueSubmissionsQueueUpdate(submission.GetCourseId(), submission.GetGroupId());
        
        return submission;
    }
}