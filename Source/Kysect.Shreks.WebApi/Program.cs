using Kysect.Shreks.Application.Commands.Extensions;
using Kysect.Shreks.Application.Handlers.Extensions;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Core.SubjectCourseAssociations;
using Kysect.Shreks.Core.UserAssociations;
using Kysect.Shreks.Core.Users;
using Kysect.Shreks.DataAccess.Abstractions;
using Kysect.Shreks.DataAccess.Extensions;
using Kysect.Shreks.Integration.Github.Extensions;
using Kysect.Shreks.Integration.Github.Helpers;
using Kysect.Shreks.Integration.Google.Extensions;
using Kysect.Shreks.Mapping.Extensions;
using Kysect.Shreks.Seeding.EntityGenerators;
using Kysect.Shreks.Seeding.Extensions;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog(
    (ctx, lc) => lc.MinimumLevel.Verbose().WriteTo.Console());

ShreksConfiguration shreksConfiguration = builder.Configuration.GetShreksConfiguration();
TestEnvConfiguration testEnvConfiguration = builder.Configuration.GetSection(nameof(TestEnvConfiguration)).Get<TestEnvConfiguration>();

InitServiceCollection(builder);
await InitWebApplication(builder);

void InitServiceCollection(WebApplicationBuilder webApplicationBuilder)
{
    webApplicationBuilder.Services.AddControllers();
    webApplicationBuilder.Services.AddEndpointsApiExplorer();
    webApplicationBuilder.Services.AddSwaggerGen();

    webApplicationBuilder.Services
        .AddHandlers()
        .AddApplicationCommands()
        .AddMappingConfiguration();

    webApplicationBuilder.Services
        .AddDatabaseContext(opt => opt
            .UseSqlite("Filename=shreks.db")
            .UseLazyLoadingProxies());

    webApplicationBuilder.Services
        .AddGoogleCredentialsFromWeb()
        .AddGoogleIntegration();

    webApplicationBuilder.Services
        .AddGithubServices(shreksConfiguration);

    if (webApplicationBuilder.Environment.IsDevelopment())
    {
        webApplicationBuilder.Services
            .AddDatabaseSeeders()
            .AddEntityGenerators();
    }
}

async Task InitWebApplication(WebApplicationBuilder webApplicationBuilder)
{
    var app = webApplicationBuilder.Build();

    if (app.Environment.IsDevelopment())
    {
        await app.Services.UseDatabaseSeeders();
        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseCors(opt => opt.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
    }

    //app.UseHttpsRedirection();

    //app.UseAuthorization();

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

    User first = users.First();
    var login = config.Users[0];
    dbContext.UserAssociations.Add(new GithubUserAssociation(first, login));

    var subjectCourseGenerator = serviceProvider.GetRequiredService<IEntityGenerator<SubjectCourse>>();
    var subjectCourse = subjectCourseGenerator.GeneratedEntities[0];
    dbContext.SubjectCourses.Attach(subjectCourse);
    dbContext.SubjectCourseAssociations.Add(new GithubSubjectCourseAssociation(subjectCourse, config.Organization));

    await dbContext.SaveChangesAsync(cancellationToken);
}

public class TestEnvConfiguration
{
    public string Organization { get; init; }
    public List<string> Users { get; init; }
}