using System.Globalization;
using Kysect.Shreks.Abstractions;

namespace Kysect.Shreks.GoogleIntegration.Extensions;

public static class EnumerableExtensions
{
    private static readonly CultureInfo CurrentCultureInfo = new("ru-RU");

    public static IEnumerable<object> GetPointsData(
        this IEnumerable<AssignmentPoints> assignmentPoints)
    {
        double totalPoints = 0;

        foreach (AssignmentPoints assignmentPoint in assignmentPoints)
        {
            yield return PointsToSheetData(assignmentPoint.Points);
            yield return DateToSheetData(assignmentPoint.Date);
            totalPoints += assignmentPoint.Points;
        }

        yield return PointsToString(totalPoints);
    }

    private static string PointsToSheetData(double points)
        => points == 0 ? string.Empty : PointsToString(points);

    private static string PointsToString(double points)
        => Math.Round(points, 2).ToString(CurrentCultureInfo);

    private static string DateToSheetData(DateOnly? dateOnly)
        => dateOnly?.ToString("dd.MM.yyyy") ?? string.Empty;
}