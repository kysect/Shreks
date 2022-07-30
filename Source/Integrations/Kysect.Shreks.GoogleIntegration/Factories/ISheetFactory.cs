using Kysect.Shreks.GoogleIntegration.Models;
using Kysect.Shreks.GoogleIntegration.Sheets;

namespace Kysect.Shreks.GoogleIntegration.Factories;

public interface ISheetFactory<out TSheet>
    where TSheet : ISheet
{
    TSheet Create(CreateSheetArguments sheetArguments);
}