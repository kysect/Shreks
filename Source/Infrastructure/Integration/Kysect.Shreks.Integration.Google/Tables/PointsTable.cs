using FluentSpreadsheets;
using FluentSpreadsheets.Tables;
using Kysect.Shreks.Application.Abstractions.Formatters;
using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Application.Dto.Tables;
using Kysect.Shreks.Integration.Google.Extensions;
using Kysect.Shreks.Integration.Google.Providers;
using System.Globalization;
using static FluentSpreadsheets.ComponentFactory;

namespace Kysect.Shreks.Integration.Google.Tables;

public class PointsTable : RowTable<CoursePointsDto>, ITableCustomizer
{
    private static readonly IComponent EmptyAssignmentPointsCell = HStack(Enumerable.Repeat(Empty(), 2));

    private readonly IUserFullNameFormatter _userFullNameFormatter;
    private readonly ICultureInfoProvider _cultureInfoProvider;

    public PointsTable(IUserFullNameFormatter userFullNameFormatter, ICultureInfoProvider cultureInfoProvider)
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
            Label("ФИО").WithColumnWidth(240),
            Label("Группа"),
            ForEach(points.Assignments, a => VStack
            (
                Label(a.ShortName),
                HStack
                (
                    Label("Балл"),
                    Label("Дата")
                )
            )).CustomizedWith(g => VStack(Label("Лабораторные"), g)),
            Label("Итог")
        );

        CultureInfo currentCulture = _cultureInfoProvider.GetCultureInfo();

        foreach (var (student, assignmentPoints) in points.StudentsPoints)
        {
            double totalPoints = assignmentPoints.Sum(p => p.Points);
            double roundedPoints = Math.Round(totalPoints, 2);

            yield return Row
            (
                Label(_userFullNameFormatter.GetFullName(student.User)), 
                Label(student.GroupName),
                ForEach(points.Assignments, a =>
                    BuildAssignmentPointsCell(a, assignmentPoints, currentCulture)),
                Label(roundedPoints, currentCulture)
            );
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
            Label(assignmentPoints.Points, formatProvider),
            Label(assignmentPoints.Date, formatProvider)
        );
    }
}