using FluentSpreadsheets;
using FluentSpreadsheets.Tables;
using Kysect.Shreks.Application.Abstractions.Formatters;
using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Application.Dto.Tables;
using Kysect.Shreks.Integration.Google.Extensions;
using Kysect.Shreks.Integration.Google.Providers;
using System.Drawing;
using System.Globalization;
using static FluentSpreadsheets.ComponentFactory;

namespace Kysect.Shreks.Integration.Google.Tables;

public class LabsTable : RowTable<CoursePointsDto>, ITableCustomizer
{
    private static readonly IComponent BlankLabel = Label(string.Empty);

    private static readonly IComponent EmptyAssignmentPointsCell = HStack
    (
        BlankLabel.WithLeadingMediumBorder(),
        BlankLabel.WithTrailingMediumBorder()
    );

    private readonly IUserFullNameFormatter _userFullNameFormatter;
    private readonly ICultureInfoProvider _cultureInfoProvider;

    public LabsTable(IUserFullNameFormatter userFullNameFormatter, ICultureInfoProvider cultureInfoProvider)
    {
        _userFullNameFormatter = userFullNameFormatter;
        _cultureInfoProvider = cultureInfoProvider;
    }

    public IComponent Customize(IComponent component)
        => component.WithDefaultStyle();
    
    protected override IEnumerable<IRowComponent> RenderRows(CoursePointsDto points)
    {
        yield return Row
        (
            Label("ISU").WithColumnWidth(60),
            Label("ФИО").WithColumnWidth(240),
            Label("Группа"),
            Label("GitHub").WithColumnWidth(150).Frozen(),
            ForEach(points.Assignments, a => VStack
            (
                Label(a.ShortName).WithSideMediumBorder(),
                HStack
                (
                    Label("Балл").WithLeadingMediumBorder(),
                    Label("Дата").WithTrailingMediumBorder()
                )
            )).CustomizedWith(g =>
                VStack(Label("Лабораторные").WithSideMediumBorder().WithBottomMediumBorder(), g)),
            Label("Итог").WithTrailingMediumBorder()
        );

        CultureInfo currentCulture = _cultureInfoProvider.GetCultureInfo();

        IList<StudentPointsDto> studentPoints = points.StudentsPoints.ToArray();

        for (int i = 0; i < studentPoints.Count; i++)
        {
            var (student, assignmentPoints) = studentPoints[i];

            double totalPoints = assignmentPoints.Sum(p => p.Points);
            double roundedPoints = Math.Round(totalPoints, 2);

            var row = Row
            (
                Label(student.UniversityId),
                Label(_userFullNameFormatter.GetFullName(student.User)),
                Label(student.GroupName),
                Label(student.GitHubUsername!),
                ForEach(points.Assignments, a =>
                    BuildAssignmentPointsCell(a, assignmentPoints, currentCulture)),
                Label(roundedPoints, currentCulture).WithTrailingMediumBorder()
            );
            
            if (i % 2 is 0)
                row = row.FilledWith(Color.AliceBlue);

            if (i is 0 || student.GroupName != studentPoints[i - 1].Student.GroupName)
                row = row.WithTopMediumBorder();

            if (i == studentPoints.Count - 1)
                row = row.WithBottomMediumBorder();

            yield return row;
        }
    }

    private static IComponent BuildAssignmentPointsCell(
        AssignmentDto assignment,
        IEnumerable<AssignmentPointsDto> points,
        IFormatProvider formatProvider)
    {
        var assignmentPoints = points.FirstOrDefault(p => p.AssignmentId == assignment.Id);

        if (assignmentPoints is null)
            return EmptyAssignmentPointsCell;

        return HStack
        (
            Label(assignmentPoints.Points, formatProvider).WithLeadingMediumBorder(),
            Label(assignmentPoints.Date, formatProvider).WithTrailingMediumBorder()
        );
    }
}