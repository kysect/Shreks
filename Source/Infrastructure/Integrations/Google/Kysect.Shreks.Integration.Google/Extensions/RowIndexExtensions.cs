using Kysect.Centum.Sheets.Indices;

namespace Kysect.Shreks.Integration.Google.Extensions;

public static class RowIndexExtensions
{
    public static bool IsNotNone(this RowIndex rowIndex)
        => rowIndex != RowIndex.None;
}