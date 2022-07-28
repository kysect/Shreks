using Kysect.Shreks.Abstractions;

namespace Kysect.Shreks.GoogleIntegration.Extensions.Entities;

public static class StudentPointsExtensions
{
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
            double assignmentPoints = Math.Round(p.Points, 2);
            data.Add(assignmentPoints != 0 ? assignmentPoints : "");
            data.Add(p.Date?.ToString("dd.MM.yyyy") ?? "");
        });

        var totalPoints = Math.Round(points.Sum(p => p.Points), 2);
        data.Add(totalPoints);

        return data;
    }

    public static string GetStudentIdentifier(this StudentPoints studentPoints)
        => $"{studentPoints.Student.GetFullName()}{studentPoints.Student.Group.Name}";
}