using Kysect.Shreks.Core.Study;
using Kysect.Shreks.GoogleIntegration.Converters;
using Kysect.Shreks.GoogleIntegration.Models;
using Kysect.Shreks.GoogleIntegration.Sheets;

namespace Kysect.Shreks.GoogleIntegration.Factories;

public class QueueSheetFactory : ISheetFactory<QueueSheet>
{
    private readonly ISheetDataConverter<Submission> _submissionConverter;
    
    public QueueSheetFactory(ISheetDataConverter<Submission> submissionConverter)
    {
        _submissionConverter = submissionConverter;
    }

    public QueueSheet Create(CreateSheetArguments sheetArguments)
    {
        return new QueueSheet(_submissionConverter, sheetArguments);
    }
}