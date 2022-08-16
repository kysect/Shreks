using Kysect.Shreks.Application.Abstractions.Google;
using Kysect.Shreks.Application.Abstractions.Google.Models;
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

    public Task UpdatePointsAsync(string spreadsheetId, CoursePoints points, CancellationToken token = default)
        => _pointsSheet.UpdateAsync(spreadsheetId, points, token);

    public Task UpdateQueueAsync(string spreadsheetId, StudentsQueue queue, CancellationToken token = default)
        => _queueSheet.UpdateAsync(spreadsheetId, queue, token);
}