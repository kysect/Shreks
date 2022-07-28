using Google.Apis.Sheets.v4.Data;
using Kysect.Shreks.Abstractions;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.GoogleIntegration.Attributes;
using Kysect.Shreks.GoogleIntegration.Exceptions;
using Kysect.Shreks.GoogleIntegration.Extensions.Entities;
using Kysect.Shreks.GoogleIntegration.Models;
using Kysect.Shreks.GoogleIntegration.Tools;

namespace Kysect.Shreks.GoogleIntegration.Sheets;

[GoogleSheet("Баллы", "A1:Z3", "A4:Q")]
public class PointsSheet : ISheet
{
    private const int FrozenColumnCount = 1;
    private const int AssignmentColumnsStart = 2;

    private static readonly IList<object> InitialHeader = new List<object>
    {
        "ФИО",
        "Группа",
        "Лабораторные работы"
    };

    private static readonly IReadOnlyCollection<string> InitialMergeRanges = new[]
    {
        "A1:A3",
        "B1:B3"
    };

    private static readonly IReadOnlyCollection<int> ColumnLengths = new[] { 240 };

    public int Id { get; init; }

    public SheetRange HeaderRange { get; init; } = null!;
    public SheetRange DataRange { get; init; } = null!;

    public GoogleTableEditor Editor { get; init; } = null!;

    public async Task UpdatePointsAsync(IReadOnlyCollection<StudentPoints> points, CancellationToken token)
    {
        await Editor.ClearValuesAsync(DataRange, token);

        List<StudentPoints> sortedPoints = points
            .OrderBy(p => p.Student.Group.Name)
            .ThenBy(p => p.Student.GetFullName())
            .ToList();

        IEnumerable<Assignment> assignments = sortedPoints
                                                  .FirstOrDefault()?
                                                  .Points
                                                  .Select(p => p.Assignment) 
                                              ?? Enumerable.Empty<Assignment>();

        await FormatAsync(assignments.ToList(), token);

        IList<IList<object>> values = sortedPoints
            .Select(s => s.ToSheetData())
            .ToList();
        
        await Editor.SetValuesAsync(values, DataRange, token);
    }

    public async Task UpdateStudentPointsAsync(StudentPoints studentPoints, CancellationToken token)
    {
        string studentIdentifier = studentPoints.Student.GetSheetIdentifier();
        IList<object> newStudentRow = studentPoints.ToSheetData();

        IList<IList<object>> values = await Editor.GetValuesAsync(DataRange, token);
        IList<object>? studentRow = values.FirstOrDefault(row =>
        {
            string rowIdentifier = $"{row[0]}{row[1]}";
            return rowIdentifier == studentIdentifier;
        });

        if (studentRow is null)
        {
            throw new GoogleTableException(
                $"Student with identifier {studentIdentifier} was not found");
        }

        int studentIndex = values.IndexOf(studentRow);
        values[studentIndex] = newStudentRow;
        
        await Editor.SetValuesAsync(values, DataRange, token);
    }

    private async Task FormatAsync(IList<Assignment> assignments, CancellationToken token)
    {
        await Editor.ClearValuesAsync(HeaderRange, token);
        await Editor.SetAlignmentAsync(HeaderRange, token);
        await Editor.FreezeRowsAsync(HeaderRange, token);
        await Editor.FreezeColumnsAsync(new SheetProperties
        {
            SheetId = Id,
            GridProperties = new GridProperties
            {
                FrozenColumnCount = FrozenColumnCount
            }
        }, token);
        await Editor.MergeCellsAsync(GetMergeRanges(assignments.Count), token);
        await Editor.SetValuesAsync(GetHeader(assignments), HeaderRange, token);
        await Editor.ResizeColumnsAsync(ColumnLengths, Id, token);
    }

    private IList<GridRange> GetMergeRanges(int assignmentCount)
    {
        IList<GridRange> mergeRanges = InitialMergeRanges
            .Select(r => new SheetRange(Id, r).GridRange)
            .ToList();
        
        int assignmentColumnEnd = AssignmentColumnsStart + assignmentCount * 2;

        mergeRanges.Add(new GridRange
        {
            StartColumnIndex = AssignmentColumnsStart,
            EndColumnIndex = assignmentColumnEnd,
            StartRowIndex = 0,
            EndRowIndex = 1,
            SheetId = Id
        });

        for (int i = AssignmentColumnsStart; i < assignmentColumnEnd; i += 2)
        {
            mergeRanges.Add(new GridRange
            {
                StartColumnIndex = i,
                EndColumnIndex = i + 2,
                StartRowIndex = 1,
                EndRowIndex = 2,
                SheetId = Id
            });
        }

        return mergeRanges;
    }

    private static IList<IList<object>> GetHeader(IList<Assignment> assignments)
    {
        var header = new List<IList<object>>
        {
            InitialHeader,
            GetListOfEmptyStrings(2),
            GetListOfEmptyStrings(2)
        };

        foreach (Assignment assignment in assignments)
        {
            header[1].Add(assignment.Title);
            header[1].Add("");

            header[2].Add("Балл");
            header[2].Add("Дата");
        }

        header[2].Add("Итог");

        return header;
    }

    private static IList<object> GetListOfEmptyStrings(int emptyStringsCount)
        => new List<object>(Enumerable.Repeat("", emptyStringsCount));
}
