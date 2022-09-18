namespace Kysect.Shreks.Application.TableManagement;

public interface ITableAccessor : IDisposable
{
    Task UpdatePointsAsync(Guid subjectCourseId, CancellationToken token);
    Task UpdateQueueAsync(Guid subjectCourseId, Guid studentGroupId, CancellationToken token);
}