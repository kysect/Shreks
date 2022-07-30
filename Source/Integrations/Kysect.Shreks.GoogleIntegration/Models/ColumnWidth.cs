using Google.Apis.Sheets.v4.Data;
using Kysect.Centum.Sheets.Models;
using Kysect.Shreks.GoogleIntegration.SheetEnums;

namespace Kysect.Shreks.GoogleIntegration.Models;

public record struct ColumnWidth(int ColumnIndex, int Width)
{
    public Request GetResizeColumnRequest(int sheetId)
    {
        var dimensionRange = new DimensionRange
        {
            StartIndex = ColumnIndex,
            EndIndex = ColumnIndex + 1,
            Dimension = Dimension.Columns,
            SheetId = sheetId
        };
        
        var updateProperties = new UpdateDimensionPropertiesRequest
        {
            Properties = new DimensionProperties
            {
                PixelSize = Width
            },
            Range = dimensionRange,
            Fields = UpdatedFields.All
        };

        return new Request
        {
            UpdateDimensionProperties = updateProperties
        };
    }
}