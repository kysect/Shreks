using FluentSpreadsheets;
using FluentSpreadsheets.SheetBuilders;
using FluentSpreadsheets.SheetSegments;
using Kysect.Shreks.Application.Dto.Tables;
using Kysect.Shreks.Integration.Google.Segments;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Kysect.Shreks.Integration.Google.Factories;

public class QueueSheetComponentFactory : ISheetComponentFactory<SubmissionsQueueDto>
{
    private readonly ISheetBuilder _sheetBuilder;
    private readonly ISheetSegment<Unit, QueueSubmissionDto, Unit>[] _segments;

    public QueueSheetComponentFactory(
        ISheetBuilder sheetBuilder,
        IServiceProvider serviceProvider)
    {
        _sheetBuilder = sheetBuilder;

        _segments = new ISheetSegment<Unit, QueueSubmissionDto, Unit>[]
        {
            ActivatorUtilities.CreateInstance<QueueStudentSegment>(serviceProvider),
            ActivatorUtilities.CreateInstance<AssignmentDataSegment>(serviceProvider)
        };
    }

    public IComponent Create(SubmissionsQueueDto queue)
    {
        var sheetData = new SheetData<Unit, QueueSubmissionDto, Unit>(Unit.Value, queue.Submissions, Unit.Value);
        return _sheetBuilder.Build(_segments, sheetData);
    }
}