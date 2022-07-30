using Kysect.Shreks.GoogleIntegration.Models;
using Kysect.Shreks.GoogleIntegration.Tools;

namespace Kysect.Shreks.GoogleIntegration.Sheets;

public interface ISheet
{
    int Id { get; }

    SheetRange HeaderRange { get; }
    SheetRange DataRange { get; }
    
    GoogleTableEditor Editor { get; }
}