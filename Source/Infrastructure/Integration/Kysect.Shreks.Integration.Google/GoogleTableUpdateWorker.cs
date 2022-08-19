using Kysect.Shreks.Application.Abstractions.Google;
using Kysect.Shreks.Integration.Google.Models;
using Microsoft.Extensions.Hosting;

namespace Kysect.Shreks.Integration.Google;

public class GoogleTableUpdateWorker : BackgroundService, ITableUpdateQueue
{
    private static readonly TimeSpan DelayBetweenSheetUpdates = TimeSpan.FromMinutes(1);

    private readonly ConcurrentHashSet<Guid> _queueUpdateSubjectCourseIds;
    private readonly ConcurrentHashSet<Guid> _pointsUpdateSubjectCourseIds;

    private readonly GoogleTableAccessor _tableAccessor;

    public GoogleTableUpdateWorker(GoogleTableAccessor tableAccessor)
    {
        _tableAccessor = tableAccessor;

        _queueUpdateSubjectCourseIds = new ConcurrentHashSet<Guid>();
        _pointsUpdateSubjectCourseIds = new ConcurrentHashSet<Guid>();
    }

    protected override async Task ExecuteAsync(CancellationToken token)
    {
        using var timer = new PeriodicTimer(DelayBetweenSheetUpdates);
        while (!token.IsCancellationRequested && await timer.WaitForNextTickAsync(token))
        {
            var pointsUpdateTasks = _pointsUpdateSubjectCourseIds
                .GetAndClearValues()
                .Select(i => _tableAccessor.UpdatePointsAsync(i, token));

            var queueUpdateTasks = _queueUpdateSubjectCourseIds
                .GetAndClearValues()
                .Select(i => _tableAccessor.UpdateQueueAsync(i, token));

            await Task.WhenAll(pointsUpdateTasks.Concat(queueUpdateTasks));
        }
    }

    public void EnqueueCoursePointsUpdate(Guid subjectCourseId)
        => _pointsUpdateSubjectCourseIds.Add(subjectCourseId);

    public void EnqueueSubmissionsQueueUpdate(Guid subjectCourseId)
        => _queueUpdateSubjectCourseIds.Add(subjectCourseId);
}