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

    public async Task SetAlignmentAsync(SheetRange sheetRange, CancellationToken token)
        => await SetAlignmentAsync(sheetRange, HorizontalAlignment.Center, VerticalAlignment.Middle, token);

    public async Task SetAlignmentAsync(
        SheetRange sheetRange,
        HorizontalAlignment horizontalAlignment,
        VerticalAlignment verticalAlignment,
        CancellationToken token)
        => await ExecuteBatchUpdateAsync(new Request 
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

    public async Task FreezeRowsAsync(SheetRange sheetRange, CancellationToken token)
        => await FreezeRowsAsync(sheetRange.FrozenRowProperties, token);

    public async Task FreezeRowsAsync(SheetProperties sheetProperties, CancellationToken token)
        => await ExecuteBatchUpdateAsync(new Request
        {
            UpdateSheetProperties = new UpdateSheetPropertiesRequest
            {
                Properties = sheetProperties,
                Fields = UpdatedFields.FrozenRowCount
            }
        }, token);

    public async Task FreezeColumnsAsync(SheetProperties sheetProperties, CancellationToken token)
        => await ExecuteBatchUpdateAsync(new Request 
        {
            UpdateSheetProperties = new UpdateSheetPropertiesRequest
            {
                Properties = sheetProperties,
                Fields = UpdatedFields.FrozenColumnCount
            }
        }, token);

    public async Task ResizeColumnsAsync(IEnumerable<int> orderedLengths, int sheetId, CancellationToken token)
        => await ExecuteBatchUpdateAsync(orderedLengths.Select((l, i) => new Request 
        {
            UpdateDimensionProperties = new UpdateDimensionPropertiesRequest
            {
                Properties = new DimensionProperties
                {
                    PixelSize = l
                },
                Range = new DimensionRange
                {
                    StartIndex = i,
                    EndIndex = i + 1,
                    Dimension = Dimension.Columns,
                    SheetId = sheetId
                },
                Fields = UpdatedFields.All
            }
        }).ToList(), token);

    public async Task<IList<IList<object>>> GetValuesAsync(SheetRange sheetRange, CancellationToken token)
        => (await _service.Spreadsheets.Values
            .Get(_spreadsheetId, sheetRange.Range)
            .ExecuteAsync(token))
            .Values;

    public async Task SetValuesAsync(IList<object> values, SheetRange sheetRange, CancellationToken token)
        => await SetValuesAsync(new List<IList<object>> { values }, sheetRange, token);

    public async Task SetValuesAsync(IList<IList<object>> values, SheetRange sheetRange, CancellationToken token)
        => await _service.Spreadsheets.Values.BatchUpdate(new BatchUpdateValuesRequest
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

    public async Task ClearValuesAsync(SheetRange sheetRange, CancellationToken token)
        => await ExecuteBatchUpdateAsync(new Request
        {
            UpdateCells = new UpdateCellsRequest
            {
                Range = sheetRange.GridRange,
                Fields = UpdatedFields.All
            }
        }, token);

    private async Task ExecuteBatchUpdateAsync(IList<Request> requests, CancellationToken token)
        => await _service.Spreadsheets.BatchUpdate(
                new BatchUpdateSpreadsheetRequest
                {
                    Requests = requests
                },
                _spreadsheetId)
            .ExecuteAsync(token);

    private async Task ExecuteBatchUpdateAsync(Request request, CancellationToken token)
        => await ExecuteBatchUpdateAsync(new List<Request> { request }, token);
}