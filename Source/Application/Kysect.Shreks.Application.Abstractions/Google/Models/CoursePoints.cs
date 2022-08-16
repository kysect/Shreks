using Kysect.Shreks.Core.Study;

namespace Kysect.Shreks.Application.Abstractions.Google.Models;

public record struct CoursePoints(
    IReadOnlyCollection<Assignment> Assignments,
    IReadOnlyCollection<StudentPoints> StudentsPoints);