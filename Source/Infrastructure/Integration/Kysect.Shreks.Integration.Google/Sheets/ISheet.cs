namespace Kysect.Shreks.Integration.Google.Sheets;

public interface ISheet<in TModel>
{
    Task UpdateAsync(string spreadsheetId, TModel model, CancellationToken token);
}