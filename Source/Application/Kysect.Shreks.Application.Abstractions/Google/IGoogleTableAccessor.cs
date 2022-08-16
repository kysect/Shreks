using Kysect.Shreks.Application.Abstractions.Google.Models;

namespace Kysect.Shreks.Application.Abstractions.Google;

public interface IGoogleTableAccessor
{
    Task UpdateQueueAsync(string spreadsheetId, StudentsQueue queue, CancellationToken token = default);
    Task UpdatePointsAsync(string spreadsheetId, CoursePoints points, CancellationToken token = default);
}