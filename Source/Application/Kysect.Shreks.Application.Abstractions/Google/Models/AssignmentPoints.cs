using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Core.ValueObject;

namespace Kysect.Shreks.Application.Abstractions.Google.Models;

public record AssignmentPoints(Assignment Assignment, DateOnly Date, Points Points);