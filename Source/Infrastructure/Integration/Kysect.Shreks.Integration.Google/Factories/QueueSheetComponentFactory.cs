using FluentSpreadsheets;
using FluentSpreadsheets.SheetBuilders;
using FluentSpreadsheets.SheetSegments;
using Kysect.Shreks.Application.Abstractions.Google;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Integration.Google.Segments;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Kysect.Shreks.Integration.Google.Factories;

public class QueueSheetComponentFactory : ISheetComponentFactory<StudentsQueue>
{
    private readonly ISheetBuilder _sheetBuilder;
    private readonly ISheetSegment<Unit, Submission, Unit>[] _segments;

    public QueueSheetComponentFactory(
        ISheetBuilder sheetBuilder,
        IServiceProvider serviceProvider)
    {
        _sheetBuilder = sheetBuilder;

        _segments = new ISheetSegment<Unit, Submission, Unit>[]
        {
            ActivatorUtilities.CreateInstance<QueueStudentSegment>(serviceProvider),
            ActivatorUtilities.CreateInstance<AssignmentDataSegment>(serviceProvider)
        };
    }

    public IComponent Create(StudentsQueue queue)
    {
        var sheetData = new SheetData<Unit, Submission, Unit>(Unit.Value, queue.Submissions, Unit.Value);
        return _sheetBuilder.Build(_segments, sheetData);
    }
}