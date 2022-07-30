using Kysect.Shreks.GoogleIntegration.Tools;

namespace Kysect.Shreks.GoogleIntegration.Models;

public record struct CreateSheetArguments(
    int SheetId,
    SheetRange HeaderSheetRange,
    SheetRange DataSheetRange,
    GoogleTableEditor Editor);