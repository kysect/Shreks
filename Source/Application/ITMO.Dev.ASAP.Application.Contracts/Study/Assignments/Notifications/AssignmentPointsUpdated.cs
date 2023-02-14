using ITMO.Dev.ASAP.Application.Dto.Study;
using MediatR;

namespace ITMO.Dev.ASAP.Application.Contracts.Study.Assignments.Notifications;

internal static class AssignmentPointsUpdated
{
    public record Notification(AssignmentDto Assignment) : INotification;
}