using ITMO.Dev.ASAP.Application.Abstractions.Google;
using ITMO.Dev.ASAP.Application.Google.Tools;

namespace ITMO.Dev.ASAP.Application.Google.Services;

public class TableUpdateQueue : ITableUpdateQueue
{
    public TableUpdateQueue()
    {
        QueueUpdateSubjectCourseGroupIds = new ConcurrentHashSet<(Guid, Guid)>();
        PointsUpdateSubjectCourseIds = new ConcurrentHashSet<Guid>();
    }

    public ConcurrentHashSet<(Guid SubjectCourseId, Guid StudentGroupId)> QueueUpdateSubjectCourseGroupIds { get; }

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