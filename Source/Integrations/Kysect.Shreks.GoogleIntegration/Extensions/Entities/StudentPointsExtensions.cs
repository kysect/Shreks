using System.Globalization;
using Kysect.Shreks.Abstractions;

namespace Kysect.Shreks.GoogleIntegration.Extensions.Entities;

internal static class StudentPointsExtensions
{
    private static readonly CultureInfo CurrentCultureInfo = new("ru-RU");

    public static IList<object> ToSheetData(this StudentPoints studentPoints)
    {
        var data = new List<object>
        {
            studentPoints.Student.GetFullName(),
            studentPoints.Student.Group.Name
        };

        List<AssignmentPoints> points = studentPoints.Points
            .OrderBy(p => p.Assignment.Id)
            .ToList();

        points.ForEach(p =>
        {
            data.Add(PointsToSheetData(p.Points));
            data.Add(DateToSheetData(p.Date));
        });

        double totalPoints = points.Sum(p => p.Points);
        data.Add(PointsToString(totalPoints));

        return data;
    }

    private static string PointsToSheetData(double points)
        => points == 0 ? "" : PointsToString(points);

    private static string PointsToString(double points)
        => Math.Round(points, 2).ToString(CurrentCultureInfo);

    private static string DateToSheetData(DateOnly? dateOnly)
        => dateOnly?.ToString("dd.MM.yyyy") ?? "";
}