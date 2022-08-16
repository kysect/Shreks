using Kysect.Shreks.Core.Users;

namespace Kysect.Shreks.Application.Abstractions.Google.Models;

public record StudentPoints(Student Student, IReadOnlyCollection<AssignmentPoints> Points);