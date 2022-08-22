namespace Kysect.Shreks.Integration.Google.Sheets;

public interface ISheet<in TData>
{
    Task UpdateAsync(string spreadsheetId, TData data, CancellationToken token);
}