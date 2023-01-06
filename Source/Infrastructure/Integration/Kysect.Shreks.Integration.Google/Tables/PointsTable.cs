using FluentSpreadsheets;
using FluentSpreadsheets.GoogleSheets.Extensions;
using FluentSpreadsheets.Tables;
using Kysect.Shreks.Application.Dto.Tables;
using Kysect.Shreks.Integration.Google.Extensions;
using Kysect.Shreks.Integration.Google.Models;
using Kysect.Shreks.Integration.Google.Sheets;
using static FluentSpreadsheets.ComponentFactory;
using Index = FluentSpreadsheets.Index;

namespace Kysect.Shreks.Integration.Google.Tables;

public class PointsTable : RowTable<CourseStudentsDto>
{
    private const string ReferenceSheetTitle = LabsSheet.Title;
    private const int ReferenceRowShift = 2;
    private const string ReferenceHeaderRange = "1:1";

    private static readonly IRowComponent Header = Row(
        Label("ISU").WithColumnWidth(0.8).WithFrozenRows(),
        Label("ФИО").WithColumnWidth(2),
        Label("Группа"),
        Label("GitHub").WithColumnWidth(1.2),
        Label("Лабораторные").WithColumnWidth(1.1),
        Label("Karma"),
        Label("Экзамен"),
        Label("Сумма"),
        Label("Оценка"),
        Label("Комментарий").WithColumnWidth(2.7).WithTrailingMediumBorder());

    protected override IComponent Customize(IComponent component)
    {
        return component.WithDefaultStyle();
    }

    protected override IEnumerable<IRowComponent> RenderRows(CourseStudentsDto model)
    {
        yield return Header;

        IReadOnlyList<StudentPointsDto> studentPoints = model.StudentsPoints.ToArray();

        for (int i = 0; i < studentPoints.Count; i++)
        {
            IRowComponent row = GetRowReference()
                .WithDefaultStyle(i, studentPoints.Count)
                .WithGroupSeparators(i, studentPoints);

            yield return row;
        }
    }

    private static IRowComponent GetRowReference()
    {
        return Row(
            Label(AssignedReference),
            Label(AssignedReference),
            Label(AssignedReference),
            Label(AssignedReference),
            Label(i => Index("\"Итог\"", ReferenceHeaderRange, i.Row)),
            Empty(),
            Empty(),
            Label(GetTotalFunction),
            Label(PointsFormula),
            Empty().WithTrailingMediumBorder());
    }

    private static string AssignedReference(Index index)
    {
        index = index.WithRowShift(ReferenceRowShift);
        return $"={index.ToGoogleSheetsIndex(ReferenceSheetTitle)}";
    }

    private static string Index(string value, string fieldsRange, int row)
    {
        row += ReferenceRowShift;
        string rowRange = $"{row}:{row}";
        return $"=INDEX({GetCellsReference(rowRange)},1,MATCH({value},{GetCellsReference(fieldsRange)},0))";
    }

    private static string GetCellsReference(string range)
    {
        return $"{ReferenceSheetTitle}!{range}";
    }

    private static string GetTotalFunction(Index index)
    {
        string labsPoints = GetCellNumber(index.WithColumnShift(-3));
        string karmaPoints = GetCellNumber(index.WithColumnShift(-2));
        string examPoints = GetCellNumber(index.WithColumnShift(-1));
        string total = $"{labsPoints}+{karmaPoints}+{examPoints}";
        return $"=SUBSTITUTE({total},\".\",\",\")";
    }

    private static string GetCellNumber(Index index)
    {
        string i = index.ToGoogleSheetsIndex();
        return $"IFERROR(VALUE(SUBSTITUTE({i},\",\",\".\")), VALUE(SUBSTITUTE({i},\".\",\",\")))";
    }

    private static string PointsFormula(Index index)
    {
        string n = GetCellNumber(index.WithColumnShift(-1));
        return
            $"=if({n}>=60,if({n}>67,if({n}>74,if({n}>83,if({n}>90,\"A\",\"B\"),\"C\"),\"D\"),\"E\"),if({n}>=40,\"FX-E\",\"FX\"))";
    }
}