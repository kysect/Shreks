using Bogus;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Kysect.Shreks.Application.Abstractions.Google;
using Kysect.Shreks.Core.Formatters;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Core.Users;
using Kysect.Shreks.Integration.Google.Extensions;
using Kysect.Shreks.Integration.Google.Providers;
using Kysect.Shreks.Seeding.EntityGenerators;
using Kysect.Shreks.Seeding.Extensions;
using Microsoft.Extensions.DependencyInjection;

var credential = await GoogleCredential.FromFileAsync("client_secrets.json", default);

var initializer = new BaseClientService.Initializer
{
    HttpClientInitializer = credential
};

var sheetsService = new SheetsService(initializer);

const string spreadSheetId = "";

var spreadsheetIdProvider = new ConstSpreadsheetIdProvider(spreadSheetId);

IServiceProvider services = new ServiceCollection()
    .AddGoogleIntegration()
    .AddSingleton(sheetsService)
    .AddSingleton<ISpreadsheetIdProvider>(spreadsheetIdProvider)
    .AddSingleton<IUserFullNameFormatter, UserFullNameFormatter>()
    .AddSingleton<ICultureInfoProvider, RuCultureInfoProvider>()
    .AddEntityGenerators(o =>
    {
        o.ConfigureEntityGenerator<Submission>(s => s.Count = 300); 
        o.ConfigureEntityGenerator<Student>(s => s.Count = 100); 
        o.ConfigureEntityGenerator<Assignment>(s => s.Count = 5);
        o.Faker = new Faker("ru");
    })
    .BuildServiceProvider();

var googleTableAccessor = services.GetRequiredService<IGoogleTableAccessor>();

var submissionGenerator = services.GetRequiredService<IEntityGenerator<Submission>>();
IReadOnlyCollection<Submission> submissions = submissionGenerator.GeneratedEntities;

var queue = new StudentsQueue(submissions);
await googleTableAccessor.UpdateQueueAsync(queue);

var studentGenerator = services.GetRequiredService<IEntityGenerator<Student>>();
var assignmentGenerator = services.GetRequiredService<IEntityGenerator<Assignment>>();

IReadOnlyCollection<Assignment> assignments = assignmentGenerator.GeneratedEntities;

IReadOnlyCollection<StudentPoints> studentPoints = studentGenerator.GeneratedEntities
    .Select(s => new StudentPoints(s, GetPoints(s, submissions)))
    .ToList();

var points = new Points(assignments, studentPoints);
await googleTableAccessor.UpdatePointsAsync(points);

IReadOnlyCollection<AssignmentPoints> GetPoints(Student student, IReadOnlyCollection<Submission> submissions)
{
    return submissions
        .Where(sub => sub.Student.Equals(student))
        .GroupBy(sub => sub.Assignment)
        .Select(sub => sub.MaxBy(g => g.SubmissionDateTime))
        .Where(sub => sub is not null)
        .Select(GetAssignmentPoints!)
        .ToList();
}

AssignmentPoints GetAssignmentPoints(Submission submission)
{
    return new AssignmentPoints(
        submission.Assignment,
        DateOnly.FromDateTime(submission.SubmissionDateTime),
        submission.Points);
}