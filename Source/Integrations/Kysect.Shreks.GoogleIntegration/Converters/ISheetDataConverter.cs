namespace Kysect.Shreks.GoogleIntegration.Converters;

public interface ISheetDataConverter<in TEntity>
{
    IList<object> GetSheetData(TEntity entity);
}