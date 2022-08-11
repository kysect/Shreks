using FluentSpreadsheets;

namespace Kysect.Shreks.Integration.Google.Factories;

public interface ISheetDataFactory<in TData>
{
    IComponent Create(TData data);
}