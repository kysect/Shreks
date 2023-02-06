using Kysect.Shreks.Application.Abstractions.Permissions;
using Kysect.Shreks.Application.Contracts.Study.Submissions.Notifications;
using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Application.Extensions;
using Kysect.Shreks.Core.Submissions;
using Kysect.Shreks.DataAccess.Abstractions;
using Kysect.Shreks.DataAccess.Abstractions.Extensions;
using Kysect.Shreks.Mapping.Mappings;
using MediatR;
using static Kysect.Shreks.Application.Contracts.Study.Submissions.Commands.DeleteSubmission;

namespace Kysect.Shreks.Application.Handlers.Study.Submissions;

internal class DeleteSubmissionHandler : IRequestHandler<Command, Response>
{
    private readonly IShreksDatabaseContext _context;
    private readonly IPermissionValidator _permissionValidator;
    private readonly IPublisher _publisher;

    public DeleteSubmissionHandler(
        IPermissionValidator permissionValidator,
        IShreksDatabaseContext context,
        IPublisher publisher)
    {
        _permissionValidator = permissionValidator;
        _context = context;
        _publisher = publisher;
    }

    public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
    {
        await _permissionValidator.EnsureSubmissionMentorAsync(
            request.IssuerId,
            request.SubmissionId,
            cancellationToken);

        Submission submission = await _context.Submissions
            .IncludeSubjectCourse()
            .IncludeStudentGroup()
            .GetByIdAsync(request.SubmissionId, cancellationToken);

        submission.Delete();

        _context.Submissions.Update(submission);
        await _context.SaveChangesAsync(cancellationToken);

        SubmissionDto dto = submission.ToDto();

        var notification = new SubmissionUpdated.Notification(dto);
        await _publisher.PublishAsync(notification, cancellationToken);

        return new Response(dto);
    }
}