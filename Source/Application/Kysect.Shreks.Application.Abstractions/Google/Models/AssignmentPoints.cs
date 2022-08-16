using Kysect.Shreks.Core.Study;

namespace Kysect.Shreks.Application.Abstractions.Google.Models;

public record AssignmentPoints(Assignment Assignment, DateOnly Date, double Points);