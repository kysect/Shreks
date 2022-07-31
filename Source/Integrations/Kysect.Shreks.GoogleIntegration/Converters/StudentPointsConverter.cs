using System.Globalization;
using Kysect.Shreks.Abstractions;
using Kysect.Shreks.Core.Formatters;

namespace Kysect.Shreks.GoogleIntegration.Converters;

public class StudentPointsConverter : ISheetDataConverter<StudentPoints>
{
    private static readonly CultureInfo CurrentCultureInfo = new("ru-RU");

    private readonly IUserFullNameFormatter _userFullNameFormatter;

    public StudentPointsConverter(IUserFullNameFormatter userFullNameFormatter)
    {
        _userFullNameFormatter = userFullNameFormatter;
    }

    public IList<object> GetSheetData(StudentPoints studentPoints)
    {
        IEnumerable<object> data = Enumerable
            .Repeat(_userFullNameFormatter.GetFullName(studentPoints.Student), 1)
            .Append(studentPoints.Student.Group.Name);

        IEnumerable<AssignmentPoints> points = studentPoints.Points
            .OrderBy(p => p.Assignment.Id);
        
        return data
            .Concat(GetPointsData(points))
            .ToList();
    }

    public static IEnumerable<object> GetPointsData(
        IEnumerable<AssignmentPoints> assignmentPoints)
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