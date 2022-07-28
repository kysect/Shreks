using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Core.Users;

namespace Kysect.Shreks.Abstractions;

public record AssignmentPoints(Assignment Assignment, DateOnly? Date, double Points);

public record StudentPoints(Student Student, IEnumerable<AssignmentPoints> Points);


public interface IGoogleTableAccessor
{
    Task UpdateQueueAsync(IEnumerable<Submission> submissions, CancellationToken token = default);
    Task UpdatePointsAsync(IEnumerable<StudentPoints> points, CancellationToken token = default);
    Task UpdateStudentPointsAsync(StudentPoints studentPoints, CancellationToken token = default);
}