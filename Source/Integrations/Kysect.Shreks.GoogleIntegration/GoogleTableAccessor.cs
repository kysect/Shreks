using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Kysect.Shreks.Abstractions;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.GoogleIntegration.Sheets;
using Kysect.Shreks.GoogleIntegration.Tools;

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

        var service = new SheetsService(
            new BaseClientService.Initializer
            {
                HttpClientInitializer = credential
            });

        var sheetCreator = new GoogleSheetCreator(service, spreadsheetId);

        var pointsSheet = await sheetCreator.GetOrCreateSheetAsync<PointsSheet>(token);
        var queueSheet = await sheetCreator.GetOrCreateSheetAsync<QueueSheet>(token);
        
        return new GoogleTableAccessor(pointsSheet, queueSheet);
    }

    public async Task UpdateQueueAsync(
        IReadOnlyCollection<Submission> submissions,
        CancellationToken token = default)
        => await _queueSheet.UpdateQueueAsync(submissions, token);

    public async Task UpdatePointsAsync(
        IReadOnlyCollection<StudentPoints> points,
        CancellationToken token = default)
        => await _pointsSheet.UpdatePointsAsync(points, token);
    
    public async Task UpdateStudentPointsAsync(
        StudentPoints studentPoints,
        CancellationToken token = default)
        => await _pointsSheet.UpdateStudentPointsAsync(studentPoints, token);
}