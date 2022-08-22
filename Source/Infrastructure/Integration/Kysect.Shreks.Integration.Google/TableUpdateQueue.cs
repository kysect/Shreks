using Kysect.Shreks.Application.Abstractions.Google;
using Kysect.Shreks.Integration.Google.Models;

namespace Kysect.Shreks.Integration.Google;

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
        => QueueUpdateSubjectCourseGroupIds.Add((subjectCourseId, studentGroupId));

    public void EnqueueCoursePointsUpdate(Guid subjectCourseId)
        => PointsUpdateSubjectCourseIds.Add(subjectCourseId);
}