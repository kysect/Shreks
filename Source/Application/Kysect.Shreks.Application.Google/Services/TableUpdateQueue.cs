using Kysect.Shreks.Application.Abstractions.Google;
using Kysect.Shreks.Application.Google.Tools;

namespace Kysect.Shreks.Application.Google.Services;

public class TableUpdateQueue : ITableUpdateQueue
{
    public TableUpdateQueue()
    {
        QueueUpdateSubjectCourseGroupIds = new ConcurrentHashSet<(Guid, Guid)>();
        PointsUpdateSubjectCourseIds = new ConcurrentHashSet<Guid>();
    }

    public ConcurrentHashSet<(Guid, Guid)> QueueUpdateSubjectCourseGroupIds { get; }
    public ConcurrentHashSet<Guid> PointsUpdateSubjectCourseIds { get; }

    public void EnqueueSubmissionsQueueUpdate(Guid subjectCourseId, Guid studentGroupId)
    {
        QueueUpdateSubjectCourseGroupIds.Add((subjectCourseId, studentGroupId));
    }

    public void EnqueueCoursePointsUpdate(Guid subjectCourseId)
    {
        PointsUpdateSubjectCourseIds.Add(subjectCourseId);
    }
}