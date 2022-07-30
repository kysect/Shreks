using Google.Apis.Sheets.v4.Data;
using Kysect.Shreks.Abstractions;
using Kysect.Shreks.Core.Formatters;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.GoogleIntegration.Converters;
using Kysect.Shreks.GoogleIntegration.Exceptions;
using Kysect.Shreks.GoogleIntegration.Models;
using Kysect.Shreks.GoogleIntegration.Providers;
using Kysect.Shreks.GoogleIntegration.Tools;

namespace Kysect.Shreks.GoogleIntegration.Sheets;

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

    private static readonly IReadOnlyCollection<ColumnWidth> ColumnLengths
        = new ColumnWidth[] { new(0, 240) };

    private readonly IUserFullNameFormatter _userFullNameFormatter;
    private readonly IStudentIdentifierProvider _studentIdentifierProvider;
    private readonly ISheetDataConverter<StudentPoints> _studentPointsConverter;

    public PointsSheet(
        IUserFullNameFormatter userFullNameFormatter,
        IStudentIdentifierProvider studentIdentifierProvider,
        ISheetDataConverter<StudentPoints> studentPointsConverter,
        CreateSheetArguments sheetArguments)
    {
        _userFullNameFormatter = userFullNameFormatter;
        _studentIdentifierProvider = studentIdentifierProvider;
        _studentPointsConverter = studentPointsConverter;
        (Id, HeaderRange, DataRange, Editor) = sheetArguments;
    }

    public static SheetDescriptor Descriptor { get; }
        = new("Баллы", "A1:Z3", "A4:Q");

    public int Id { get; }

    public SheetRange HeaderRange { get; }
    public SheetRange DataRange { get; }

    public GoogleTableEditor Editor { get; }

    public async Task UpdatePointsAsync(IReadOnlyCollection<StudentPoints> points, CancellationToken token)
    {
        await Editor.ClearValuesAsync(DataRange, token);

        List<StudentPoints> sortedPoints = points
            .OrderBy(p => p.Student.Group.Name)
            .ThenBy(p => _userFullNameFormatter.GetFullName(p.Student))
            .ToList();

        IList<Assignment> assignments = GetAssignments(sortedPoints);

        await FormatAsync(assignments, token);

        IList<IList<object>> values = sortedPoints
            .Select(_studentPointsConverter.GetSheetData)
            .ToList();
        
        await Editor.SetValuesAsync(values, DataRange, token);
    }

    public async Task UpdateStudentPointsAsync(StudentPoints studentPoints, CancellationToken token)
    {
        string studentIdentifier = _studentIdentifierProvider.GetStudentIdentifier(studentPoints.Student);
        IList<object> newStudentRow = _studentPointsConverter.GetSheetData(studentPoints);

        IList<IList<object>> values = await Editor.GetValuesAsync(DataRange, token);

        IList<object>? studentRow = values.FirstOrDefault(row =>
        {
            string rowIdentifier = $"{row[0]}{row[1]}";
            return rowIdentifier == studentIdentifier;
        });

        if (studentRow is null)
        {
            throw new GoogleTableException($"Student with identifier {studentIdentifier} was not found");
        }

        int studentIndex = values.IndexOf(studentRow);
        values[studentIndex] = newStudentRow;
        
        await Editor.SetValuesAsync(values, DataRange, token);
    }

    private async Task FormatAsync(IList<Assignment> assignments, CancellationToken token)
    {
        var freezeColumnProperties = new SheetProperties
        {
            SheetId = Id,
            GridProperties = new GridProperties
            {
                FrozenColumnCount = FrozenColumnCount
            }
        };

        await Editor.ClearValuesAsync(HeaderRange, token);
        await Editor.SetAlignmentAsync(HeaderRange, token);
        await Editor.FreezeRowsAsync(HeaderRange, token);
        await Editor.FreezeColumnsAsync(freezeColumnProperties, token);
        await Editor.MergeCellsAsync(GetMergeRanges(assignments.Count), token);
        await Editor.SetValuesAsync(GetHeader(assignments), HeaderRange, token);
        await Editor.ResizeColumnsAsync(ColumnLengths, Id, token);
    }

    private IList<GridRange> GetMergeRanges(int assignmentCount)
    {
        IList<GridRange> mergeRanges = InitialMergeRanges
            .Select(r => new SheetRange(Descriptor.Title, Id, r).GridRange)
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

    private static IList<Assignment> GetAssignments(IEnumerable<StudentPoints> sortedPoints)
    {
        StudentPoints? studentPoint = sortedPoints.FirstOrDefault();

        if (studentPoint is null)
        {
            return Array.Empty<Assignment>();
        }

        return studentPoint
            .Points
            .Select(p => p.Assignment)
            .ToList();
    }

    private static IList<object> GetListOfEmptyStrings(int emptyStringsCount)
        => new List<object>(Enumerable.Repeat("", emptyStringsCount));
}
