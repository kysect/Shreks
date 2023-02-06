using Kysect.Shreks.Application.Dto.Study;
using MediatR;

namespace Kysect.Shreks.Application.Contracts.Study.Assignments.Notifications;

internal static class AssignmentPointsUpdated
{
    public record Notification(AssignmentDto Assignment) : INotification;
}