using Kysect.Shreks.Application.Abstractions.Google;
using Kysect.Shreks.Integration.Google.Sheets;

namespace Kysect.Shreks.Integration.Google;

public class GoogleTableAccessor : IGoogleTableAccessor
{
    private readonly ISheet<CoursePoints> _pointsSheet;
    private readonly ISheet<StudentsQueue> _queueSheet;

    public GoogleTableAccessor(ISheet<CoursePoints> pointsSheet, ISheet<StudentsQueue> queueSheet)
    {
        _pointsSheet = pointsSheet;
        _queueSheet = queueSheet;
    }

    public Task UpdatePointsAsync(CoursePoints points, CancellationToken token = default)
        => _pointsSheet.UpdateAsync(points, token);

    public Task UpdateQueueAsync(StudentsQueue queue, CancellationToken token = default)
        => _queueSheet.UpdateAsync(queue, token);
}