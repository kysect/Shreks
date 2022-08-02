using Kysect.Shreks.Core.Formatters;
using Kysect.Shreks.Integration.Google.Converters;
using Kysect.Shreks.Integration.Google.Models;
using Kysect.Shreks.Integration.Google.Sheets;

namespace Kysect.Shreks.Integration.Google.Factories;

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