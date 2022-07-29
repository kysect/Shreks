using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Kysect.Centum.Sheets.Models;
using Kysect.Shreks.GoogleIntegration.Models;
using Kysect.Shreks.GoogleIntegration.SheetEnums;

namespace Kysect.Shreks.GoogleIntegration.Tools;

public class GoogleTableEditor
{
    private readonly SheetsService _service;
    private readonly string _spreadsheetId;

    public GoogleTableEditor(SheetsService service, string spreadsheetId)
    {
        _service = service;
        _spreadsheetId = spreadsheetId;
    }

    public Task SetAlignmentAsync(SheetRange sheetRange, CancellationToken token)
        => SetAlignmentAsync(sheetRange, HorizontalAlignment.Center, VerticalAlignment.Middle, token);

    public Task SetAlignmentAsync(
        SheetRange sheetRange,
        HorizontalAlignment horizontalAlignment,
        VerticalAlignment verticalAlignment,
        CancellationToken token)
        => ExecuteBatchUpdateAsync(new Request 
        {
            RepeatCell = new RepeatCellRequest
            {
                Cell = new CellData
                {
                    UserEnteredFormat = new CellFormat
                    {
                        HorizontalAlignment = horizontalAlignment,
                        VerticalAlignment = verticalAlignment
                    }
                },
                Range = sheetRange.GridRange,
                Fields = UpdatedFields.All
            }
        }, token);

    public async Task MergeCellsAsync(IList<GridRange> ranges, CancellationToken token)
    {
        await ExecuteBatchUpdateAsync(ranges.Select(r =>
            new Request 
            {
                MergeCells = new MergeCellsRequest
                {
                    Range = r,
                    MergeType = MergeType.All
                }
            }
        ).ToList(), token);
    }

    public Task FreezeRowsAsync(SheetRange sheetRange, CancellationToken token)
        => FreezeRowsAsync(sheetRange.FrozenRowProperties, token);

    public Task FreezeRowsAsync(SheetProperties sheetProperties, CancellationToken token)
        => ExecuteBatchUpdateAsync(new Request
        {
            UpdateSheetProperties = new UpdateSheetPropertiesRequest
            {
                Properties = sheetProperties,
                Fields = UpdatedFields.FrozenRowCount
            }
        }, token);

    public Task FreezeColumnsAsync(SheetProperties sheetProperties, CancellationToken token)
        => ExecuteBatchUpdateAsync(new Request 
        {
            UpdateSheetProperties = new UpdateSheetPropertiesRequest
            {
                Properties = sheetProperties,
                Fields = UpdatedFields.FrozenColumnCount
            }
        }, token);

    public Task ResizeColumnsAsync(IEnumerable<ColumnWidth> columnWidths, int sheetId, CancellationToken token)
        => ExecuteBatchUpdateAsync(columnWidths.Select(c => 
            c.GetUpdateColumnWidthRequest(sheetId))
            .ToList(), token);

    public async Task<IList<IList<object>>> GetValuesAsync(SheetRange sheetRange, CancellationToken token)
        => (await _service.Spreadsheets.Values
            .Get(_spreadsheetId, sheetRange.Range)
            .ExecuteAsync(token))
            .Values;

    public Task SetValuesAsync(IList<object> values, SheetRange sheetRange, CancellationToken token)
        => SetValuesAsync(new List<IList<object>> { values }, sheetRange, token);

    public Task SetValuesAsync(IList<IList<object>> values, SheetRange sheetRange, CancellationToken token)
        => _service.Spreadsheets.Values.BatchUpdate(new BatchUpdateValuesRequest
        {
            Data = new List<ValueRange>
            {
                new()
                {
                    Values = values,
                    Range = sheetRange.Range
                }
            },
            ValueInputOption = ValueInputOption.UserEntered

        }, _spreadsheetId).ExecuteAsync(token);

    public Task ClearValuesAsync(SheetRange sheetRange, CancellationToken token)
        => ExecuteBatchUpdateAsync(new Request
        {
            UpdateCells = new UpdateCellsRequest
            {
                Range = sheetRange.GridRange,
                Fields = UpdatedFields.All
            }
        }, token);

    private Task ExecuteBatchUpdateAsync(IList<Request> requests, CancellationToken token)
        => _service.Spreadsheets.BatchUpdate(
                new BatchUpdateSpreadsheetRequest
                {
                    Requests = requests
                },
                _spreadsheetId)
            .ExecuteAsync(token);

    private Task ExecuteBatchUpdateAsync(Request request, CancellationToken token)
        => ExecuteBatchUpdateAsync(new List<Request> { request }, token);
}