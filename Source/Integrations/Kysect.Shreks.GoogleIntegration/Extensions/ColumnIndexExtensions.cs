using Kysect.Centum.Sheets.Indices;

namespace Kysect.Shreks.GoogleIntegration.Extensions;

public static class ColumnIndexExtensions
{
    public static bool IsNotNone(this ColumnIndex columnIndex)
        => columnIndex != ColumnIndex.None;
}