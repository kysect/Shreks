using Kysect.Shreks.Application.Abstractions.GoogleSheets;
using Kysect.Shreks.Integration.Google.Sheets;

namespace Kysect.Shreks.Integration.Google;

public class GoogleTableAccessor : IGoogleTableAccessor
{
    private readonly ISheet<Points> _pointsSheet;
    private readonly ISheet<Queue> _queueSheet;

    public GoogleTableAccessor(ISheet<Points> pointsSheet, ISheet<Queue> queueSheet)
    {
        _pointsSheet = pointsSheet;
        _queueSheet = queueSheet;
    }

    public Task UpdatePointsAsync(Points points, CancellationToken token = default)
        => _pointsSheet.UpdateAsync(points, token);

    public Task UpdateQueueAsync(Queue queue, CancellationToken token = default)
        => _queueSheet.UpdateAsync(queue, token);
}