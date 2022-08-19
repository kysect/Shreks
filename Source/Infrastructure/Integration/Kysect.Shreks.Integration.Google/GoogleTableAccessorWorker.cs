using Kysect.Shreks.Application.Abstractions.Google;
using Kysect.Shreks.Integration.Google.Models;
using Microsoft.Extensions.Hosting;

namespace Kysect.Shreks.Integration.Google;

public class GoogleTableAccessorWorker : BackgroundService, IGoogleTableAccessor
{
    private static readonly TimeSpan DelayBetweenSheetUpdates = TimeSpan.FromMinutes(1);

    private readonly ConcurrentHashSet<Guid> _queueUpdateSubjectCourseIds;
    private readonly ConcurrentHashSet<Guid> _pointsUpdateSubjectCourseIds;

    private readonly IGoogleTableAccessor _tableAccessor;

    public GoogleTableAccessorWorker(IGoogleTableAccessor tableAccessor)
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

            await Task.WhenAll(pointsUpdateTasks.Union(queueUpdateTasks));
        }
    }

    public Task UpdatePointsAsync(Guid subjectCourseId, CancellationToken token = default)
    {
        _pointsUpdateSubjectCourseIds.Add(subjectCourseId);
        return Task.CompletedTask;
    }

    public Task UpdateQueueAsync(Guid subjectCourseId, CancellationToken token = default)
    {
        _queueUpdateSubjectCourseIds.Add(subjectCourseId);
        return Task.CompletedTask;
    }
}