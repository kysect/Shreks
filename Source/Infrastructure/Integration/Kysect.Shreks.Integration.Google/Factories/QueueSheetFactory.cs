using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Integration.Google.Converters;
using Kysect.Shreks.Integration.Google.Models;
using Kysect.Shreks.Integration.Google.Sheets;

namespace Kysect.Shreks.Integration.Google.Factories;

public class QueueSheetFactory : ISheetFactory<QueueSheet>
{
    private readonly ISheetRowConverter<Submission> _submissionConverter;
    
    public QueueSheetFactory(ISheetRowConverter<Submission> submissionConverter)
    {
        _submissionConverter = submissionConverter;
    }

    public QueueSheet Create(CreateSheetArguments sheetArguments)
    {
        return new QueueSheet(_submissionConverter, sheetArguments);
    }
}