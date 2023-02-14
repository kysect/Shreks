using ITMO.Dev.ASAP.Application.Dto.Study;
using MediatR;

namespace ITMO.Dev.ASAP.Application.Contracts.Study.GroupAssignments.Notifications;

internal static class GroupAssignmentDeadlineUpdated
{
    public record Notification(GroupAssignmentDto GroupAssignment) : INotification;
}