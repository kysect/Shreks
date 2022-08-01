using Kysect.Shreks.Integration.Google.Models;
using Kysect.Shreks.Integration.Google.Tools;

namespace Kysect.Shreks.Integration.Google.Sheets;

public interface ISheet
{
    int Id { get; }

    SheetRange HeaderRange { get; }
    SheetRange DataRange { get; }
    
    GoogleTableEditor Editor { get; }
}