﻿using FluentSpreadsheets;
using FluentSpreadsheets.GoogleSheets.Extensions;
using FluentSpreadsheets.Tables;
using Kysect.Shreks.Integration.Google.Extensions;
using Kysect.Shreks.Integration.Google.Sheets;
using static FluentSpreadsheets.ComponentFactory;
using Index = FluentSpreadsheets.Index;

namespace Kysect.Shreks.Integration.Google.Tables;

public class PointsTable : RowTable<int>, ITableCustomizer
{
    private const string ReferenceSheetTitle = $"'{LabsSheet.Title}'";
    private const int ReferenceRowShift = 2;
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

    protected override IEnumerable<IRowComponent> RenderRows(int studentsCount)
    {
        yield return Header;

        foreach (var row in Enumerable.Range(1, studentsCount).Select(GetRowReference))
        {
            yield return row;
        }
    }

    private static IRowComponent GetRowReference(int row)
    {
        return Row
        (
            Label(AssignedReference),
            Label(AssignedReference),
            Label(AssignedReference),
            Label(AssignedReference),
            Label(i => LookUp("\"Итог\"", ReferenceHeaderRange, i.Row)),
            Empty(),
            Empty(),
            Label(GetTotalFunction),
            Label(PointsFormula),
            Empty()
        );
    }

    private static string AssignedReference(Index index)
    {
        index = index.WithRowShift(ReferenceRowShift);
        return $"={index.ToGoogleSheetsIndex(ReferenceSheetTitle)}";
    }

    private static string LookUp(string value, string fieldsRange, int row)
    {
        row += ReferenceRowShift;
        string rowRange = $"{row}:{row}";
        return $"=LOOKUP({value},{GetCellsReference(fieldsRange)},{GetCellsReference(rowRange)})";
    }

    private static string GetCellsReference(string range)
        => $"{ReferenceSheetTitle}!{range}";

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
        return $"=if({n}>=60,if({n}>67,if({n}>74,if({n}>83,if({n}>90,\"A\",\"B\"),\"C\"),\"D\"),\"E\"),if({n}>= 40,\"FX-E\",\"FX\"))";
    }
}