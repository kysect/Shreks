namespace Kysect.Shreks.Integration.Google.Providers;

public class ConstSpreadsheetIdProvider : ISpreadsheetIdProvider
{
    private readonly string _spreadsheetId;

    public ConstSpreadsheetIdProvider(string spreadSheetId)
    {
        _spreadsheetId = spreadSheetId;
    }

    public string GetSpreadsheetId()
        => _spreadsheetId;
}