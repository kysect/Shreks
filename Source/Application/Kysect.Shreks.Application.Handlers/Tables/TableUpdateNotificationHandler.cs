using Kysect.Shreks.Application.Abstractions.Google;
using Kysect.Shreks.Application.Contracts.Study.Assignments.Notifications;
using Kysect.Shreks.Application.Contracts.Study.GroupAssignments.Notifications;
using Kysect.Shreks.Application.Contracts.Study.StudyGroups.Notifications;
using Kysect.Shreks.Application.Contracts.Study.SubjectCourseGroups.Notifications;
using Kysect.Shreks.Application.Contracts.Study.SubjectCourses.Notifications;
using Kysect.Shreks.Application.Contracts.Study.Submissions.Notifications;
using Kysect.Shreks.Application.Extensions;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.DataAccess.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Kysect.Shreks.Application.Handlers.Tables;

internal class TableUpdateNotificationHandler :
    INotificationHandler<AssignmentCreated.Notification>,
    INotificationHandler<AssignmentPointsUpdated.Notification>,
    INotificationHandler<GroupAssignmentDeadlineUpdated.Notification>,
    INotificationHandler<StudyGroupUpdated.Notification>,
    INotificationHandler<SubjectCourseGroupCreated.Notification>,
    INotificationHandler<SubjectCourseGroupDeleted.Notification>,
    INotificationHandler<DeadlinePolicyAdded.Notification>,
    INotificationHandler<SubmissionPointsUpdated.Notification>,
    INotificationHandler<SubmissionStateUpdated.Notification>,
    INotificationHandler<SubmissionUpdated.Notification>
{
    private readonly ITableUpdateQueue _tableUpdateQueue;
    private readonly IShreksDatabaseContext _context;

    public TableUpdateNotificationHandler(ITableUpdateQueue tableUpdateQueue, IShreksDatabaseContext context)
    {
        _tableUpdateQueue = tableUpdateQueue;
        _context = context;
    }

    public Task Handle(AssignmentCreated.Notification notification, CancellationToken cancellationToken)
    {
        _tableUpdateQueue.EnqueueCoursePointsUpdate(notification.Assignment.SubjectCourseId);
        return Task.CompletedTask;
    }

    public Task Handle(AssignmentPointsUpdated.Notification notification, CancellationToken cancellationToken)
    {
        _tableUpdateQueue.EnqueueCoursePointsUpdate(notification.Assignment.SubjectCourseId);
        return Task.CompletedTask;
    }

    public async Task Handle(
        GroupAssignmentDeadlineUpdated.Notification notification,
        CancellationToken cancellationToken)
    {
        SubjectCourse subjectCourse = await _context.GetSubjectCourseByAssignmentId(
            notification.GroupAssignment.AssignmentId,
            cancellationToken);

        _tableUpdateQueue.EnqueueCoursePointsUpdate(subjectCourse.Id);
        _tableUpdateQueue.EnqueueSubmissionsQueueUpdate(subjectCourse.Id, notification.GroupAssignment.GroupId);
    }

    public async Task Handle(StudyGroupUpdated.Notification notification, CancellationToken cancellationToken)
    {
        List<SubjectCourse> courses = await _context.SubjectCourses
            .Where(sc => sc.Groups.Any(g => g.StudentGroupId.Equals(notification.Group.Id)))
            .ToListAsync(cancellationToken);

        foreach (SubjectCourse course in courses)
        {
            _tableUpdateQueue.EnqueueCoursePointsUpdate(course.Id);
        }
    }

    public Task Handle(SubjectCourseGroupCreated.Notification notification, CancellationToken cancellationToken)
    {
        (Guid subjectCourseId, Guid groupId) = notification.Group;
        _tableUpdateQueue.EnqueueSubmissionsQueueUpdate(subjectCourseId, groupId);

        return Task.CompletedTask;
    }

    public Task Handle(SubjectCourseGroupDeleted.Notification notification, CancellationToken cancellationToken)
    {
        _tableUpdateQueue.EnqueueSubmissionsQueueUpdate(notification.SubjectCourseId, notification.GroupId);
        return Task.CompletedTask;
    }

    public Task Handle(DeadlinePolicyAdded.Notification notification, CancellationToken cancellationToken)
    {
        _tableUpdateQueue.EnqueueCoursePointsUpdate(notification.SubjectCourseId);
        return Task.CompletedTask;
    }

    public async Task Handle(SubmissionPointsUpdated.Notification notification, CancellationToken cancellationToken)
    {
        SubjectCourse subjectCourse = await _context.GetSubjectCourseByAssignmentId(
            notification.Submission.AssignmentId,
            cancellationToken);

        _tableUpdateQueue.EnqueueCoursePointsUpdate(subjectCourse.Id);
    }

    public async Task Handle(SubmissionStateUpdated.Notification notification, CancellationToken cancellationToken)
    {
        SubjectCourse subjectCourse = await _context.GetSubjectCourseByAssignmentId(
            notification.Submission.AssignmentId,
            cancellationToken);

        StudentGroup group = await _context.GetStudentGroupByStudentId(
            notification.Submission.StudentId,
            cancellationToken);

        _tableUpdateQueue.EnqueueSubmissionsQueueUpdate(subjectCourse.Id, group.Id);
    }

    public async Task Handle(SubmissionUpdated.Notification notification, CancellationToken cancellationToken)
    {
        SubjectCourse subjectCourse = await _context.GetSubjectCourseByAssignmentId(
            notification.Submission.AssignmentId,
            cancellationToken);

        StudentGroup group = await _context.GetStudentGroupByStudentId(
            notification.Submission.StudentId,
            cancellationToken);

        _tableUpdateQueue.EnqueueCoursePointsUpdate(subjectCourse.Id);
        _tableUpdateQueue.EnqueueSubmissionsQueueUpdate(subjectCourse.Id, group.Id);
    }
}