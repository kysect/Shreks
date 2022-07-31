using Kysect.Shreks.GoogleIntegration.Tools;

namespace Kysect.Shreks.GoogleIntegration.Models;

public readonly record struct CreateSheetArguments(
    int SheetId,
    SheetRange HeaderSheetRange,
    SheetRange DataSheetRange,
    GoogleTableEditor Editor);