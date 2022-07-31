namespace Kysect.Shreks.GoogleIntegration.Converters;

public interface ISheetRowConverter<in TEntity>
{
    IList<object> GetSheetRow(TEntity entity);
}