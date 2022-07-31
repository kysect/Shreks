using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Core.Users;

namespace Kysect.Shreks.Abstractions;

public record AssignmentPoints(Assignment Assignment, DateOnly Date, double Points);

public record StudentPoints(Student Student, IReadOnlyCollection<AssignmentPoints> Points);

public record Points(IReadOnlyCollection<Assignment> Assignments, IReadOnlyCollection<StudentPoints> StudentsPoints);

public interface IGoogleTableAccessor
{
    Task UpdateQueueAsync(IReadOnlyCollection<Submission> submissions, CancellationToken token = default);
    Task UpdatePointsAsync(Points points, CancellationToken token = default);
}