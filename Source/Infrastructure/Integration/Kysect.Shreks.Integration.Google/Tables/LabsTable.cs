using System.Globalization;
using FluentSpreadsheets;
using FluentSpreadsheets.Tables;
using Kysect.Shreks.Application.Abstractions.Formatters;
using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Application.Dto.Tables;
using Kysect.Shreks.Application.Dto.Users;
using Kysect.Shreks.Integration.Google.Extensions;
using Kysect.Shreks.Integration.Google.Providers;
using Kysect.Shreks.Integration.Google.Tools.Comparers;
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
    {
        return component.WithDefaultStyle();
    }

    protected override IEnumerable<IRowComponent> RenderRows(CoursePointsDto model)
    {
        yield return Row
        (
            Label("ISU").WithColumnWidth(60),
            Label("ФИО").WithColumnWidth(240),
            Label("Группа"),
            Label("GitHub").WithColumnWidth(150).Frozen(),
            ForEach(model.Assignments, a => VStack
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

        IReadOnlyList<StudentPointsDto> studentPoints = model.StudentsPoints.ToArray();

        for (int i = 0; i < studentPoints.Count; i++)
        {
            (StudentDto student, IReadOnlyCollection<AssignmentPointsDto> assignmentPoints) = studentPoints[i];

            double totalPoints = assignmentPoints.Sum(p => p.Points);
            double roundedPoints = Math.Round(totalPoints, 2);

            IRowComponent row = Row
            (
                Label(student.UniversityId),
                Label(_userFullNameFormatter.GetFullName(student.User)),
                Label(student.GroupName),
                Label(student.GitHubUsername!),
                ForEach(model.Assignments, a =>
                    BuildAssignmentPointsCell(a, assignmentPoints, currentCulture)),
                Label(roundedPoints, currentCulture).WithTrailingMediumBorder()
            ).WithDefaultStyle(i, studentPoints.Count);

            if (StudentComparer.ShouldBeSeparated(student, studentPoints.ElementAtOrDefault(i - 1)?.Student))
                row = row.WithTopMediumBorder();

            yield return row;
        }
    }

    private static IComponent BuildAssignmentPointsCell(
        AssignmentDto assignment,
        IEnumerable<AssignmentPointsDto> points,
        IFormatProvider formatProvider)
    {
        AssignmentPointsDto? assignmentPoints = points.FirstOrDefault(p => p.AssignmentId == assignment.Id);

        if (assignmentPoints is null)
            return EmptyAssignmentPointsCell;

        return HStack
        (
            Label(assignmentPoints.Points, formatProvider).WithLeadingMediumBorder(),
            Label(assignmentPoints.Date, formatProvider).WithTrailingMediumBorder()
        );
    }
}