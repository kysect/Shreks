using Kysect.Shreks.GoogleIntegration.Models;
using Kysect.Shreks.GoogleIntegration.Tools;

namespace Kysect.Shreks.GoogleIntegration.Sheets;

public interface ISheet
{
    int Id { get; init; }

    SheetRange HeaderRange { get; init; }
    SheetRange DataRange { get; init; }
    
    GoogleTableEditor Editor { get; init; }
}