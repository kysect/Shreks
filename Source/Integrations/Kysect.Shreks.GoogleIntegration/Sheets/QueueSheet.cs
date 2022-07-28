using Kysect.Shreks.Core.Study;
using Kysect.Shreks.GoogleIntegration.Attributes;
using Kysect.Shreks.GoogleIntegration.Extensions.Entities;
using Kysect.Shreks.GoogleIntegration.Models;
using Kysect.Shreks.GoogleIntegration.Tools;

namespace Kysect.Shreks.GoogleIntegration.Sheets;

[GoogleSheet("Очередь", "A1:D1", "A2:D")]
public class QueueSheet : ISheet
{
    private static readonly IList<object> Header =
        new List<object>
        {
            "ФИО",
            "Группа",
            "Лабораторная работа",
            "GitHub"
        };

    private static readonly IEnumerable<int> ColumnLengths = new[]
    {
        240,
        100,
        150,
        250
    };

    private bool _sheetFormatted;

    public int Id { get; init; }

    public SheetRange HeaderRange { get; init; } = null!;
    public SheetRange DataRange { get; init; } = null!;

    public GoogleTableEditor Editor { get; init; } = null!;

    public async Task UpdateQueueAsync(IEnumerable<Submission> submissions, CancellationToken token)
    {
        if (!_sheetFormatted)
        {
            await FormatAsync(token);
            _sheetFormatted = true;
        }

        IList<IList<object>> queue = submissions.Select(s => s.ToSheetData()).ToList();

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
