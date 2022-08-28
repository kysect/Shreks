using Google.Apis.Auth.OAuth2;
using Kysect.Shreks.Application.Commands.Extensions;
using Kysect.Shreks.Application.Extensions;
using Kysect.Shreks.Application.Handlers.Extensions;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Core.SubjectCourseAssociations;
using Kysect.Shreks.Core.Submissions;
using Kysect.Shreks.Core.UserAssociations;
using Kysect.Shreks.Core.Users;
using Kysect.Shreks.DataAccess.Abstractions;
using Kysect.Shreks.DataAccess.Context;
using Kysect.Shreks.DataAccess.Extensions;
using Kysect.Shreks.Integration.Github.Extensions;
using Kysect.Shreks.Integration.Github.Helpers;
using Kysect.Shreks.Integration.Google.Extensions;
using Kysect.Shreks.Mapping.Extensions;
using Kysect.Shreks.Seeding.EntityGenerators;
using Kysect.Shreks.Seeding.Extensions;
using Kysect.Shreks.WebApi;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

ShreksConfiguration shreksConfiguration = builder.Configuration.GetShreksConfiguration();
TestEnvConfiguration testEnvConfiguration = builder.Configuration.GetSection(nameof(TestEnvConfiguration)).Get<TestEnvConfiguration>();
GoogleCredential? googleCredentials = await GoogleCredential.FromFileAsync("client_secrets.json", default);
string googleDriveId = builder.Configuration["GoogleDriveId"];

InitServiceCollection(builder);
await InitWebApplication(builder);

void InitServiceCollection(WebApplicationBuilder webApplicationBuilder)
{
    webApplicationBuilder.Services.AddControllers();
    webApplicationBuilder.Services.AddEndpointsApiExplorer();
    webApplicationBuilder.Services.AddSwaggerGen();
    webApplicationBuilder.Services.AddApplicationConfiguration();

    webApplicationBuilder.Services
        .AddHandlers()
        .AddApplicationCommands()
        .AddMappingConfiguration();

    webApplicationBuilder.Services
        .AddDatabaseContext(o => o
            .UseSqlite("Filename=shreks.db")
            .UseLazyLoadingProxies());

    if (testEnvConfiguration.UseDummyGithubImplementation)
    {
        webApplicationBuilder.Services
            .AddDummyGoogleIntegration();
    }
    else
    {
        webApplicationBuilder.Services
            .AddGoogleIntegration(o => o
                .ConfigureGoogleCredentials(googleCredentials)
                .ConfigureDriveId(googleDriveId));
    }

    webApplicationBuilder.Services
        .AddGithubServices(shreksConfiguration);

    if (webApplicationBuilder.Environment.IsDevelopment())
    {
        webApplicationBuilder.Services
            .AddEntityGenerators(o =>
            {
                o.ConfigureFaker(o => o.Locale = "ru");
                o.ConfigureEntityGenerator<User>(o => o.Count = testEnvConfiguration.Users.Count);
                o.ConfigureEntityGenerator<Student>(o => o.Count = testEnvConfiguration.Users.Count);
                o.ConfigureEntityGenerator<Mentor>(o => o.Count = testEnvConfiguration.Users.Count);
                o.ConfigureEntityGenerator<GithubUserAssociation>(o => o.Count = 0);
                o.ConfigureEntityGenerator<IsuUserAssociation>(o => o.Count = 0);
                o.ConfigureEntityGenerator<Submission>(o => o.Count = 0);
                o.ConfigureEntityGenerator<SubjectCourse>(o => o.Count = 1);
                o.ConfigureEntityGenerator<SubjectCourseAssociation>(o => o.Count = 0);
            })
            .AddDatabaseSeeders();
    }
}

async Task InitWebApplication(WebApplicationBuilder webApplicationBuilder)
{
    var app = webApplicationBuilder.Build();

    if (app.Environment.IsDevelopment())
    {
        using (IServiceScope scope = app.Services.CreateScope())
        {
            scope.ServiceProvider.GetRequiredService<ShreksDatabaseContext>().Database.EnsureDeleted();
        }
        await app.Services.UseDatabaseSeeders();
        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseCors(o => o.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
    }

    app.UseBlazorFrameworkFiles();
    app.UseStaticFiles();

    app.UseRouting();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();
    app.UseSerilogRequestLogging();

    app.UseGithubIntegration(shreksConfiguration);
    await InitTestEnvironment(app.Services, testEnvConfiguration);

    app.Run();
}

async Task InitTestEnvironment(
    IServiceProvider serviceProvider,
    TestEnvConfiguration config,
    CancellationToken cancellationToken = default)
{
    using var scope = serviceProvider.CreateScope();

    var dbContext = scope.ServiceProvider.GetRequiredService<IShreksDatabaseContext>();

    var userGenerator = serviceProvider.GetRequiredService<IEntityGenerator<User>>();
    var users = userGenerator.GeneratedEntities;
    dbContext.Users.AttachRange(users);

    for (var index = 0; index < config.Users.Count; index++)
    {
        var user = users[index];
        var login = config.Users[index];
        dbContext.UserAssociations.Add(new GithubUserAssociation(user, login));
    }


    var subjectCourseGenerator = serviceProvider.GetRequiredService<IEntityGenerator<SubjectCourse>>();
    var subjectCourse = subjectCourseGenerator.GeneratedEntities[0];
    dbContext.SubjectCourses.Attach(subjectCourse);
    dbContext.SubjectCourseAssociations.Add(new GithubSubjectCourseAssociation(subjectCourse, config.Organization));

    await dbContext.SaveChangesAsync(cancellationToken);
}