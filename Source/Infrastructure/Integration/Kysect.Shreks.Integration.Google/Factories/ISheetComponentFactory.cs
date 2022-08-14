using FluentSpreadsheets;

namespace Kysect.Shreks.Integration.Google.Factories;

public interface ISheetComponentFactory<in TData>
{
    IComponent Create(TData data);
}