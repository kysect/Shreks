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

    public async Task SetAlignmentAsync(
        SheetRange sheetRange,
        HorizontalAlignment horizontalAlignment,
        VerticalAlignment verticalAlignment,
        CancellationToken token)
    {
        var cellData = new CellData
        {
            UserEnteredFormat = new CellFormat
            {
                HorizontalAlignment = horizontalAlignment,
                VerticalAlignment = verticalAlignment
            }
        };

        var setAlignmentRequest = new Request
        {
            RepeatCell = new RepeatCellRequest
            {
                Cell = cellData,
                Range = sheetRange.GridRange,
                Fields = UpdatedFields.All
            }
        };

        await ExecuteBatchUpdateAsync(setAlignmentRequest, token);
    }

    public async Task MergeCellsAsync(IList<GridRange> ranges, CancellationToken token)
    {
        List<Request> mergeCellsRequest = ranges
            .Select(CreateMergeCellsRequest)
            .ToList();

        await ExecuteBatchUpdateAsync(mergeCellsRequest, token);
    }

    public Task FreezeRowsAsync(SheetRange sheetRange, CancellationToken token)
        => FreezeRowsAsync(sheetRange.FrozenRowProperties, token);

    public Task FreezeRowsAsync(SheetProperties sheetProperties, CancellationToken token)
        => FreezeDimensionAsync(sheetProperties, UpdatedFields.FrozenRowCount, token);

    public Task FreezeColumnsAsync(SheetProperties sheetProperties, CancellationToken token)
        => FreezeDimensionAsync(sheetProperties, UpdatedFields.FrozenColumnCount, token);

    public async Task ResizeColumnsAsync(
        IEnumerable<ColumnWidth> columnWidths,
        int sheetId,
        CancellationToken token)
    {
        List<Request> resizeColumnRequests = columnWidths
            .Select(c => CreateResizeColumnRequest(c, sheetId))
            .ToList();
            
        await ExecuteBatchUpdateAsync(resizeColumnRequests, token);
    }

    public async Task<IList<IList<object>>> GetValuesAsync(
        SheetRange sheetRange,
        CancellationToken token)
    {
        ValueRange valueRange = await _service.Spreadsheets.Values
            .Get(_spreadsheetId, sheetRange.Range)
            .ExecuteAsync(token);

        return valueRange.Values;
    }

    public Task SetValuesAsync(IList<object> values, SheetRange sheetRange, CancellationToken token)
        => SetValuesAsync(new List<IList<object>> { values }, sheetRange, token);

    public async Task SetValuesAsync(
        IList<IList<object>> values,
        SheetRange sheetRange,
        CancellationToken token)
    {
        var valueRange = new ValueRange
        {
            Values = values,
            Range = sheetRange.Range
        };

        var batchUpdateRequest = new BatchUpdateValuesRequest
        {
            Data = new List<ValueRange> { valueRange },
            ValueInputOption = ValueInputOption.UserEntered
        };

        await _service.Spreadsheets.Values
            .BatchUpdate(batchUpdateRequest, _spreadsheetId)
            .ExecuteAsync(token);
    }

    public async Task ClearValuesAsync(SheetRange sheetRange, CancellationToken token)
    {
        var updateCellsRequest = new Request
        {
            UpdateCells = new UpdateCellsRequest
            {
                Range = sheetRange.GridRange,
                Fields = UpdatedFields.All
            }
        };
        
        await ExecuteBatchUpdateAsync(updateCellsRequest, token);
    }

    private static Request CreateMergeCellsRequest(GridRange range)
    {
        return new Request
        {
            MergeCells = new MergeCellsRequest
            {
                Range = range,
                MergeType = MergeType.All
            }
        };
    }

    private static Request CreateResizeColumnRequest(ColumnWidth columnWidth, int sheetId)
    {
        var updateRequest = new UpdateDimensionPropertiesRequest
        {
            Properties = new DimensionProperties
            {
                PixelSize = columnWidth.Width
            },
            Range = columnWidth.GetDimensionRange(sheetId),
            Fields = UpdatedFields.All
        };

        return new Request
        {
            UpdateDimensionProperties = updateRequest
        };
    }


    private async Task FreezeDimensionAsync(
        SheetProperties sheetProperties,
        string fields,
        CancellationToken token)
    {
        var freezeDimensionsRequest = new Request
        {
            UpdateSheetProperties = new UpdateSheetPropertiesRequest
            {
                Properties = sheetProperties,
                Fields = fields
            }
        };

        await ExecuteBatchUpdateAsync(freezeDimensionsRequest, token);
    }

    private Task ExecuteBatchUpdateAsync(Request request, CancellationToken token)
        => ExecuteBatchUpdateAsync(new List<Request> { request }, token);

    private async Task ExecuteBatchUpdateAsync(IList<Request> requests, CancellationToken token)
    {
        var batchUpdateRequest = new BatchUpdateSpreadsheetRequest
        {
            Requests = requests
        };
        
        await _service.Spreadsheets
            .BatchUpdate(batchUpdateRequest, _spreadsheetId)
            .ExecuteAsync(token);
    }
}