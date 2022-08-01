using Kysect.Shreks.Integration.Google.Tools;

namespace Kysect.Shreks.Integration.Google.Models;

public readonly record struct CreateSheetArguments(
    int SheetId,
    SheetRange HeaderSheetRange,
    SheetRange DataSheetRange,
    GoogleTableEditor Editor);