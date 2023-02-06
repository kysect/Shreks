using Kysect.Shreks.Application.Dto.Study;
using MediatR;

namespace Kysect.Shreks.Application.Contracts.Study.Submissions.Notifications;

internal static class SubmissionPointsUpdated
{
    public record Notification(SubmissionDto Submission) : INotification;
}