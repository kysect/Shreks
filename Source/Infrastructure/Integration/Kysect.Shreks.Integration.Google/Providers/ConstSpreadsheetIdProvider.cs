namespace Kysect.Shreks.Integration.Google.Providers;

public class ConstSpreadsheetIdProvider : ISpreadsheetIdProvider
{
    public ConstSpreadsheetIdProvider(string spreadSheetId)
    {
        SpreadsheetId = spreadSheetId;
    }

    public string SpreadsheetId { get; }
}