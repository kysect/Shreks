using Google.Apis.Sheets.v4.Data;
using Kysect.Centum.Sheets.Models;
using Kysect.Shreks.GoogleIntegration.SheetEnums;

namespace Kysect.Shreks.GoogleIntegration.Models;

public class ColumnWidth
{
    private readonly int _columnIndex;
    private readonly int _width;

    public ColumnWidth(int columnIndex, int width)
    {
        _columnIndex = columnIndex;
        _width = width;
    }
    
    public Request GetUpdateColumnWidthRequest(int sheetId)
    {
        var dimensionRange = new DimensionRange
        {
            StartIndex = _columnIndex,
            EndIndex = _columnIndex + 1,
            Dimension = Dimension.Columns,
            SheetId = sheetId
        };
        
        var updateProperties = new UpdateDimensionPropertiesRequest
        {
            Properties = new DimensionProperties
            {
                PixelSize = _width
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