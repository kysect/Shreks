using Kysect.Shreks.Core.Formatters;
using Kysect.Shreks.GoogleIntegration.Converters;
using Kysect.Shreks.GoogleIntegration.Models;
using Kysect.Shreks.GoogleIntegration.Sheets;

namespace Kysect.Shreks.GoogleIntegration.Factories;

public class PointsSheetFactory : ISheetFactory<PointsSheet>
{
    private readonly IUserFullNameFormatter _userFullNameFormatter;
    private readonly ISheetRowConverter<StudentPointsArguments> _studentPointsConverter;

    public PointsSheetFactory(
        IUserFullNameFormatter userFullNameFormatter,
        ISheetRowConverter<StudentPointsArguments> studentPointsConverter)
    {
        _userFullNameFormatter = userFullNameFormatter;
        _studentPointsConverter = studentPointsConverter;
    }

    public PointsSheet Create(CreateSheetArguments sheetArguments)
    {
        return new PointsSheet(
            _userFullNameFormatter,
            _studentPointsConverter,
            sheetArguments);
    }
}