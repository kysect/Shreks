using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Kysect.Shreks.Abstractions;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.GoogleIntegration.Extensions;
using Kysect.Shreks.GoogleIntegration.Factories;
using Kysect.Shreks.GoogleIntegration.Sheets;
using Kysect.Shreks.GoogleIntegration.Tools;
using Microsoft.Extensions.DependencyInjection;

namespace Kysect.Shreks.GoogleIntegration;

public class GoogleTableAccessor : IGoogleTableAccessor
{
    private const string ClientSecretsPath = "client_secrets.json";

    private readonly PointsSheet _pointsSheet;
    private readonly QueueSheet _queueSheet;

    private GoogleTableAccessor(PointsSheet pointsSheet, QueueSheet queueSheet)
    {
        _pointsSheet = pointsSheet;
        _queueSheet = queueSheet;
    }

    public static async Task<GoogleTableAccessor> CreateAsync(
        string spreadsheetId,
        CancellationToken token = default)
    {
        var credential = await GoogleCredential.FromFileAsync(ClientSecretsPath, token);

        var initializer = new BaseClientService.Initializer
        {
            HttpClientInitializer = credential
        };

        var service = new SheetsService(initializer);

        IServiceProvider services = ConfigureServices();

        var sheetCreator = new GoogleSheetCreator(service, spreadsheetId);

        var pointsSheetFactory = services.GetRequiredService<ISheetFactory<PointsSheet>>();
        var pointsSheet = await sheetCreator.GetOrCreateSheetAsync(pointsSheetFactory, PointsSheet.Descriptor, token);

        var queueSheetFactory = services.GetRequiredService<ISheetFactory<QueueSheet>>();
        var queueSheet = await sheetCreator.GetOrCreateSheetAsync(queueSheetFactory, QueueSheet.Descriptor, token);
        
        return new GoogleTableAccessor(pointsSheet, queueSheet);
    }

    public Task UpdateQueueAsync(IReadOnlyCollection<Submission> submissions, CancellationToken token = default)
        => _queueSheet.UpdateQueueAsync(submissions, token);

    public Task UpdatePointsAsync(IReadOnlyCollection<StudentPoints> points, CancellationToken token = default)
        => _pointsSheet.UpdatePointsAsync(points, token);
    
    public Task UpdateStudentPointsAsync(StudentPoints studentPoints, CancellationToken token = default)
        => _pointsSheet.UpdateStudentPointsAsync(studentPoints, token);

    private static IServiceProvider ConfigureServices()
    {
        return new ServiceCollection()
            .AddSheetServices()
            .BuildServiceProvider();
    }
}