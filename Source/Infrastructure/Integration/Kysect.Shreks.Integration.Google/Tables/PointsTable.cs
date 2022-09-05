using FluentSpreadsheets;
using FluentSpreadsheets.Tables;
using Kysect.Shreks.Integration.Google.Extensions;
using Kysect.Shreks.Integration.Google.Sheets;
using static FluentSpreadsheets.ComponentFactory;

namespace Kysect.Shreks.Integration.Google.Tables;

public class PointsTable : RowTable<Unit>, ITableCustomizer
{
    private const string ReferenceSheetTitle = LabsSheet.Title;
    private const string ReferenceHeaderRange = "1:3";

    private static readonly IRowComponent Header = Row
    (
        Label("ISU").WithColumnWidth(60),
        Label("ФИО").WithColumnWidth(240),
        Label("Группа"),
        Label("GitHub").WithColumnWidth(150),
        Label("Лабораторные").WithColumnWidth(110),
        Label("Karma"),
        Label("Экзамен"),
        Label("Сумма"),
        Label("Оценка"),
        Label("Комментарий").WithColumnWidth(350)
    );

    public IComponent Customize(IComponent component)
        => component.WithDefaultStyle();

    protected override IEnumerable<IRowComponent> RenderRows(Unit _)
    {
        yield return Header;

        for (int i = 2; i < 1000; i++)
        {
            yield return GetRowReference(i);
        }
    }

    private static IRowComponent GetRowReference(int row)
    {
        int referenceRow = row + 2;

        return Row
        (
            Label(AssignedReference('A', referenceRow)),
            Label(AssignedReference('B', referenceRow)),
            Label(AssignedReference('C', referenceRow)),
            Label(AssignedReference('D', referenceRow)),
            Label(LookUp("\"Итог\"", ReferenceHeaderRange, referenceRow)),
            Empty(),
            Empty(),
            Label(GetTotalFunction(row)),
            Label(PointsFormula(row)),
            Empty()
        );
    }

    private static string AssignedReference(char column, int row)
    {
        string index = $"{column}{row}";
        return $"={GetCellsReference(index)}";
    }

    private static string LookUp(string value, string fieldsRange, int row)
    {
        string rowRange = $"{row}:{row}";
        return $"=LOOKUP({value},{GetCellsReference(fieldsRange)},{GetCellsReference(rowRange)})";
    }

    private static string GetTotalFunction(int row)
    {
        string total = $"{GetCellNumber('E', row)}+{GetCellNumber('F', row)}+{GetCellNumber('G', row)}";
        return $"=SUBSTITUTE({total},\".\",\",\")";
    }

    private static string GetCellNumber(char column, int row)
    {
        string index = $"{column}{row}";
        return $"IFERROR(VALUE(SUBSTITUTE({index},\",\",\".\")), VALUE(SUBSTITUTE({index},\".\",\",\")))";
    }

    private static string PointsFormula(int row)
    {
        string n = GetCellNumber('H', row);
        return $"=if({n}>=60,if({n}>67,if({n}>74,if({n}>83,if({n}>90,\"A\",\"B\"),\"C\"),\"D\"),\"E\"),if({n}>= 40,\"FX-E\",\"FX\"))";
    }

    private static string GetCellsReference(string range)
        => $"'{ReferenceSheetTitle}'!{range}";
}