namespace Kysect.Shreks.Application.Abstractions.Google;

public interface IGoogleTableAccessor
{
    Task UpdateQueueAsync(Guid subjectCourseId, CancellationToken token = default);
    Task UpdatePointsAsync(Guid subjectCourseId, CancellationToken token = default);
}