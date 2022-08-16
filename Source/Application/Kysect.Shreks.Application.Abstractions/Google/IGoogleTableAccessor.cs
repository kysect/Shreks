using Kysect.Shreks.Application.Abstractions.Google.Models;

namespace Kysect.Shreks.Application.Abstractions.Google;

public interface IGoogleTableAccessor
{
    Task UpdateQueueAsync(StudentsQueue queue, CancellationToken token = default);
    Task UpdatePointsAsync(CoursePoints points, CancellationToken token = default);
}