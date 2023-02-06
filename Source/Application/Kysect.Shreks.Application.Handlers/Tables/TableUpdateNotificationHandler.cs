using Kysect.Shreks.Application.Abstractions.Google;
using Kysect.Shreks.Application.Contracts.Study.Assignments.Notifications;
using Kysect.Shreks.Application.Contracts.Study.SubjectCourseGroups.Notifications;
using MediatR;

namespace Kysect.Shreks.Application.Handlers.Tables;

public class TableUpdateNotificationHandler :
    INotificationHandler<AssignmentCreated.Notification>,
    INotificationHandler<SubjectCourseGroupCreated.Notification>
{
    private readonly ITableUpdateQueue _tableUpdateQueue;

    public TableUpdateNotificationHandler(ITableUpdateQueue tableUpdateQueue)
    {
        _tableUpdateQueue = tableUpdateQueue;
    }

    public Task Handle(AssignmentCreated.Notification notification, CancellationToken cancellationToken)
    {
        _tableUpdateQueue.EnqueueCoursePointsUpdate(notification.Assignment.SubjectCourseId);
        return Task.CompletedTask;
    }

    public Task Handle(SubjectCourseGroupCreated.Notification notification, CancellationToken cancellationToken)
    {
        (Guid subjectCourseId, Guid groupId) = notification.Group;
        _tableUpdateQueue.EnqueueSubmissionsQueueUpdate(subjectCourseId, groupId);

        return Task.CompletedTask;
    }
}