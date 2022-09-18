using Kysect.Shreks.Application.Abstractions.Google;

namespace Kysect.Shreks.Application.TableManagement;

public class TableUpdateQueue : ITableUpdateQueue
{
    public ConcurrentHashSet<(Guid, Guid)> QueueUpdateSubjectCourseGroupIds { get; }
    public ConcurrentHashSet<Guid> PointsUpdateSubjectCourseIds { get; }

    public TableUpdateQueue()
    {
        QueueUpdateSubjectCourseGroupIds = new ConcurrentHashSet<(Guid, Guid)>();
        PointsUpdateSubjectCourseIds = new ConcurrentHashSet<Guid>();
    }

    public void EnqueueSubmissionsQueueUpdate(Guid subjectCourseId, Guid studentGroupId)
    {
        QueueUpdateSubjectCourseGroupIds.Add((subjectCourseId, studentGroupId));
    }

    public void EnqueueCoursePointsUpdate(Guid subjectCourseId)
    {
        PointsUpdateSubjectCourseIds.Add(subjectCourseId);
    }
}