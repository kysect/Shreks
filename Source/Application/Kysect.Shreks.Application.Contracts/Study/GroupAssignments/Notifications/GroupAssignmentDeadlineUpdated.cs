using Kysect.Shreks.Application.Dto.Study;
using MediatR;

namespace Kysect.Shreks.Application.Contracts.Study.GroupAssignments.Notifications;

internal static class GroupAssignmentDeadlineUpdated
{
    public record Notification(GroupAssignmentDto GroupAssignment) : INotification;
}