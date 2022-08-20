namespace Kysect.Shreks.Application.Abstractions.Google;

public interface ITableUpdateQueue
{
    void EnqueueSubmissionsQueueUpdate(Guid subjectCourseId);
    void EnqueueCoursePointsUpdate(Guid subjectCourseId);
}