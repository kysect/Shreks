using Kysect.Shreks.Abstractions;
using Kysect.Shreks.Core.Formatters;
using Kysect.Shreks.GoogleIntegration.Converters;
using Kysect.Shreks.GoogleIntegration.Models;
using Kysect.Shreks.GoogleIntegration.Providers;
using Kysect.Shreks.GoogleIntegration.Sheets;
using Microsoft.Extensions.DependencyInjection;

namespace Kysect.Shreks.GoogleIntegration.Factories;

public class PointsSheetFactory : ISheetFactory<PointsSheet>
{
    private readonly IUserFullNameFormatter _userFullNameFormatter;
    private readonly IStudentIdentifierProvider _studentIdentifierProvider;
    private readonly ISheetDataConverter<StudentPoints> _studentPointsConverter;

    public PointsSheetFactory(
        IUserFullNameFormatter userFullNameFormatter,
        IStudentIdentifierProvider studentIdentifierProvider,
        ISheetDataConverter<StudentPoints> studentPointsConverter)
    {
        _userFullNameFormatter = userFullNameFormatter;
        _studentIdentifierProvider = studentIdentifierProvider;
        _studentPointsConverter = studentPointsConverter;
    }

    public PointsSheet Create(CreateSheetArguments sheetArguments)
    {
        return new PointsSheet(
            _userFullNameFormatter,
            _studentIdentifierProvider,
            _studentPointsConverter,
            sheetArguments);
    }
}