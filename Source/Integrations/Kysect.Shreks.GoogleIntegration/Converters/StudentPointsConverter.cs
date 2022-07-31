using Kysect.Shreks.Abstractions;
using Kysect.Shreks.Core.Formatters;
using Kysect.Shreks.GoogleIntegration.Extensions;

namespace Kysect.Shreks.GoogleIntegration.Converters;

public class StudentPointsConverter : ISheetDataConverter<StudentPoints>
{
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
            .Concat(points.GetPointsData())
            .ToList();
    }
}