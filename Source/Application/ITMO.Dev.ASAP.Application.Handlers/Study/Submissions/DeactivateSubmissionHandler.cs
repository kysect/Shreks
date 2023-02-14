using ITMO.Dev.ASAP.Application.Contracts.Study.Submissions.Notifications;
using ITMO.Dev.ASAP.Application.Dto.Study;
using ITMO.Dev.ASAP.Application.Extensions;
using ITMO.Dev.ASAP.Core.Submissions;
using ITMO.Dev.ASAP.DataAccess.Abstractions;
using ITMO.Dev.ASAP.DataAccess.Abstractions.Extensions;
using ITMO.Dev.ASAP.Mapping.Mappings;
using MediatR;
using static ITMO.Dev.ASAP.Application.Contracts.Study.Submissions.Commands.DeactivateSubmission;

namespace ITMO.Dev.ASAP.Application.Handlers.Study.Submissions;

internal class DeactivateSubmissionHandler : IRequestHandler<Command, Response>
{
    private readonly IDatabaseContext _context;
    private readonly IPublisher _publisher;

    public DeactivateSubmissionHandler(
        IDatabaseContext context,
        IPublisher publisher)
    {
        _context = context;
        _publisher = publisher;
    }

    public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
    {
        Submission submission = await _context.Submissions
            .IncludeSubjectCourse()
            .IncludeStudentGroup()
            .GetByIdAsync(request.SubmissionId, cancellationToken);

        submission.Deactivate();

        _context.Submissions.Update(submission);
        await _context.SaveChangesAsync(cancellationToken);

        SubmissionDto dto = submission.ToDto();

        var notification = new SubmissionStateUpdated.Notification(dto);
        await _publisher.PublishAsync(notification, cancellationToken);

        return new Response(dto);
    }
}