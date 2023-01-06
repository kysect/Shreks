using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;

namespace Kysect.Shreks.Integration.Google.Extensions;

public static class SheetsServiceExtensions
{
    public static async Task<BatchUpdateSpreadsheetResponse> ExecuteBatchUpdateAsync(
        this SheetsService service,
        string spreadsheetId,
        Request request,
        CancellationToken token)
    {
        Request[] requests = { request };
        return await service.ExecuteBatchUpdateAsync(spreadsheetId, requests, token);
    }

    public static async Task<BatchUpdateSpreadsheetResponse> ExecuteBatchUpdateAsync(
        this SheetsService service,
        string spreadsheetId,
        IList<Request> requests,
        CancellationToken token)
    {
        var batchUpdateRequest = new BatchUpdateSpreadsheetRequest { Requests = requests };

        return await service.Spreadsheets
            .BatchUpdate(batchUpdateRequest, spreadsheetId)
            .ExecuteAsync(token);
    }
}