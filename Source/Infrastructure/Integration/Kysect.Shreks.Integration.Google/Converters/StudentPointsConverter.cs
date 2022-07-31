using System.Globalization;
using Kysect.Shreks.Application.Abstractions.GoogleSheets;
using Kysect.Shreks.Core.Formatters;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Integration.Google.Models;

namespace Kysect.Shreks.Integration.Google.Converters;

public class StudentPointsConverter : ISheetRowConverter<StudentPointsArguments>
{
    private static readonly CultureInfo CurrentCultureInfo = new("ru-RU");

    private readonly IUserFullNameFormatter _userFullNameFormatter;

    public StudentPointsConverter(IUserFullNameFormatter userFullNameFormatter)
    {
        _userFullNameFormatter = userFullNameFormatter;
    }

    public IList<object> GetSheetRow(StudentPointsArguments studentPointsArguments)
        => GetSheetRow(studentPointsArguments.Assignments, studentPointsArguments.StudentPoints).ToList();

    private IEnumerable<object> GetSheetRow(IReadOnlyCollection<Assignment> assignments, StudentPoints studentPoints)
    {
        yield return _userFullNameFormatter.GetFullName(studentPoints.Student);
        yield return studentPoints.Student.Group.Name;

        foreach (Assignment assignment in assignments)
        {
            var assignmentPoints = studentPoints.Points
                .FirstOrDefault(p => p.Assignment.Equals(assignment));

            if (assignmentPoints is null)
            {
                yield return string.Empty;
                yield return string.Empty;
            }
            else
            {
                yield return PointsToSheetData(assignmentPoints.Points);
                yield return DateToSheetData(assignmentPoints.Date);
            }
        }

        yield return PointsToString(studentPoints.Points.Sum(p => p.Points));
    }

    private static string PointsToSheetData(double points)
        => points == 0 ? string.Empty : PointsToString(points);

    private static string PointsToString(double points)
        => Math.Round(points, 2).ToString(CurrentCultureInfo);

    private static string DateToSheetData(DateOnly? dateOnly)
        => dateOnly?.ToString("dd.MM.yyyy") ?? string.Empty;
}