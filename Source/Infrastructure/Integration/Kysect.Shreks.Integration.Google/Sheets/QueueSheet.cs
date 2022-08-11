using FluentSpreadsheets;
using FluentSpreadsheets.GoogleSheets.Rendering;
using FluentSpreadsheets.Rendering;
using FluentSpreadsheets.SheetBuilders;
using FluentSpreadsheets.SheetSegments;
using Kysect.Shreks.Application.Abstractions.Google;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Integration.Google.Providers;
using Kysect.Shreks.Integration.Google.Segments;
using Kysect.Shreks.Integration.Google.Tools;
using MediatR;

namespace Kysect.Shreks.Integration.Google.Sheets;

public class QueueSheet : ISheet<Queue>
{
    private readonly ISpreadsheetIdProvider _spreadsheetIdProvider;
    private readonly ISheetController _sheetEditor;
    private readonly ISheetBuilder _sheetBuilder;
    private readonly IComponentRenderer<GoogleSheetRenderCommand> _renderer;

    private readonly ISheetSegment<Unit, Submission, Unit>[] _segments;

    public QueueSheet(
        ISpreadsheetIdProvider spreadsheetIdProvider,
        ISheetController sheetEditor,
        ISheetBuilder sheetBuilder,
        IComponentRenderer<GoogleSheetRenderCommand> renderer,
        QueueStudentSegment studentSegment,
        AssignmentDataSegment assignmentSegment)
    {
        _spreadsheetIdProvider = spreadsheetIdProvider;
        _sheetEditor = sheetEditor;
        _sheetBuilder = sheetBuilder;
        _renderer = renderer;

        _segments = new ISheetSegment<Unit, Submission, Unit>[]
        {
            studentSegment,
            assignmentSegment
        };
    }

    public string Title => "Очередь";
    public int Id => 1;

    public async Task UpdateAsync(Queue queue, CancellationToken token)
    {
        await _sheetEditor.CreateOrClearSheetAsync(this, token);

        var sheetData = new SheetData<Unit, Submission, Unit>(Unit.Value, queue.Submissions, Unit.Value);
        IComponent sheet = _sheetBuilder.Build(_segments, sheetData);

        var renderCommand = new GoogleSheetRenderCommand(_spreadsheetIdProvider.SpreadsheetId, Id, Title, sheet);
        await _renderer.RenderAsync(renderCommand, token);
    }
}
