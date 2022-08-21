using Kysect.Shreks.Application.Abstractions.Google;
using Kysect.Shreks.Integration.Google.Models;

namespace Kysect.Shreks.Integration.Google;

public class TableUpdateQueue : ITableUpdateQueue
{
    public ConcurrentHashSet<Guid> QueueUpdateSubjectCourseIds { get; }
    public ConcurrentHashSet<Guid> PointsUpdateSubjectCourseIds { get; }

    public TableUpdateQueue()
    {
        QueueUpdateSubjectCourseIds = new ConcurrentHashSet<Guid>();
        PointsUpdateSubjectCourseIds = new ConcurrentHashSet<Guid>();
    }

    public void EnqueueSubmissionsQueueUpdate(Guid subjectCourseId)
    {
        QueueUpdateSubjectCourseIds.Add(subjectCourseId);
    }

    public void EnqueueCoursePointsUpdate(Guid subjectCourseId)
    {
        PointsUpdateSubjectCourseIds.Add(subjectCourseId);
    }
}