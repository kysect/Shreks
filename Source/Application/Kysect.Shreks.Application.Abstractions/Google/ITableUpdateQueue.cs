namespace Kysect.Shreks.Application.Abstractions.Google;

public interface ITableUpdateQueue
{
    void EnqueueSubmissionsQueueUpdate(Guid subjectCourseId, Guid studentGroupId);

    void EnqueueCoursePointsUpdate(Guid subjectCourseId);
}