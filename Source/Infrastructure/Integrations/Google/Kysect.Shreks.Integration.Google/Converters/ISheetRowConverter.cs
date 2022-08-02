namespace Kysect.Shreks.Integration.Google.Converters;

public interface ISheetRowConverter<in TEntity>
{
    IList<object> GetSheetRow(TEntity entity);
}