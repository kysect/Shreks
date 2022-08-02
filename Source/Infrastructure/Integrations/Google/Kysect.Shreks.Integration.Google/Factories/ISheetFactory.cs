using Kysect.Shreks.Integration.Google.Models;
using Kysect.Shreks.Integration.Google.Sheets;

namespace Kysect.Shreks.Integration.Google.Factories;

public interface ISheetFactory<out TSheet>
    where TSheet : ISheet
{
    TSheet Create(CreateSheetArguments sheetArguments);
}