using Kysect.Shreks.Application.Abstractions.GoogleSheets;
using Kysect.Shreks.Core.Study;

namespace Kysect.Shreks.Integration.Google.Models;

public readonly record struct StudentPointsArguments(
    IReadOnlyCollection<Assignment> Assignments,
    StudentPoints StudentPoints);