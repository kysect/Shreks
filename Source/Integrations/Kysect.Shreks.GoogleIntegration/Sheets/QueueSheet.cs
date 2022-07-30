using Kysect.Shreks.Core.Study;
using Kysect.Shreks.GoogleIntegration.Converters;
using Kysect.Shreks.GoogleIntegration.Models;
using Kysect.Shreks.GoogleIntegration.Tools;

namespace Kysect.Shreks.GoogleIntegration.Sheets;

public class QueueSheet : ISheet
{
    private static readonly IList<object> Header = new List<object>
    {
        "ФИО",
        "Группа",
        "Лабораторная работа",
        "GitHub"
    };

    private static readonly IReadOnlyCollection<ColumnWidth> ColumnLengths
        = new ColumnWidth[]
        {
            new(0, 240),
            new(2, 150),
            new(3, 250)
        };

    public static SheetDescriptor Descriptor { get; }
        = new("Очередь", "A1:D1", "A2:D");

    private bool _sheetFormatted;

    private readonly ISheetDataConverter<Submission> _submissionConverter;
    public QueueSheet(
        ISheetDataConverter<Submission> submissionConverter,
        CreateSheetArguments sheetArguments)
    {
        _submissionConverter = submissionConverter;
        (Id, HeaderRange, DataRange, Editor) = sheetArguments;
    }

    public int Id { get; }

    public SheetRange HeaderRange { get; }
    public SheetRange DataRange { get; }

    public GoogleTableEditor Editor { get; }

    public async Task UpdateQueueAsync(
        IReadOnlyCollection<Submission> submissions,
        CancellationToken token)
    {
        if (!_sheetFormatted)
        {
            await FormatAsync(token);
            _sheetFormatted = true;
        }

        IList<IList<object>> queue = submissions
            .Select(_submissionConverter.GetSheetData)
            .ToList();

        await Editor.ClearValuesAsync(DataRange, token);
        await Editor.SetValuesAsync(queue, DataRange, token);
    }

    private async Task FormatAsync(CancellationToken token)
    {
        await Editor.SetAlignmentAsync(HeaderRange, token);
        await Editor.SetValuesAsync(Header, HeaderRange, token);
        await Editor.FreezeRowsAsync(HeaderRange, token);
        await Editor.ResizeColumnsAsync(ColumnLengths, Id, token);
    }
}
