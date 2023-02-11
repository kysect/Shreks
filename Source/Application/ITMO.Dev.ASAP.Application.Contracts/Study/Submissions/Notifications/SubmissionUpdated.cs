using ITMO.Dev.ASAP.Application.Dto.Study;
using MediatR;

namespace ITMO.Dev.ASAP.Application.Contracts.Study.Submissions.Notifications;

internal static class SubmissionUpdated
{
    public record Notification(SubmissionDto Submission) : INotification;
}