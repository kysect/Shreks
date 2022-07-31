using Kysect.Shreks.Abstractions;
using Kysect.Shreks.Core.Study;

namespace Kysect.Shreks.GoogleIntegration.Models;

public readonly record struct StudentPointsArguments(
    IReadOnlyCollection<Assignment> Assignments,
    StudentPoints StudentPoints);