using Google.Apis.Auth.OAuth2;
using Kysect.Shreks.Application.Abstractions.Identity.Commands;
using Kysect.Shreks.Application.Commands.Extensions;
using Kysect.Shreks.Application.Extensions;
using Kysect.Shreks.Application.Handlers.Extensions;
using Kysect.Shreks.Common.Exceptions;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Core.SubjectCourseAssociations;
using Kysect.Shreks.Core.Submissions;
using Kysect.Shreks.Core.UserAssociations;
using Kysect.Shreks.Core.Users;
using Kysect.Shreks.DataAccess.Abstractions;
using Kysect.Shreks.DataAccess.Configuration;
using Kysect.Shreks.DataAccess.Context;
using Kysect.Shreks.DataAccess.Extensions;
using Kysect.Shreks.Identity.Extensions;
using Kysect.Shreks.Integration.Github.Extensions;
using Kysect.Shreks.Integration.Github.Helpers;
using Kysect.Shreks.Integration.Google.Extensions;
using Kysect.Shreks.Integration.Google.Models;
using Kysect.Shreks.Mapping.Extensions;
using Kysect.Shreks.Playground.Github.TestEnv;
using Kysect.Shreks.Seeding.EntityGenerators;
using Kysect.Shreks.Seeding.Extensions;
using Kysect.Shreks.WebApi.Filters;
using Kysect.Shreks.WebApi.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

var googleIntegrationConfiguration = builder.Configuration.GetSection(nameof(GoogleIntegrationConfiguration)).Get<GoogleIntegrationConfiguration>();
var cacheConfiguration = builder.Configuration.GetSection(nameof(CacheConfiguration)).Get<CacheConfiguration>();
var githubIntegrationConfiguration = builder.Configuration.GetSection(nameof(GithubIntegrationConfiguration)).Get<GithubIntegrationConfiguration>();
var testEnvironmentConfiguration = builder.Configuration.GetSection(nameof(TestEnvironmentConfiguration)).Get<TestEnvironmentConfiguration>();
var postgresConfiguration = builder.Configuration.GetSection(nameof(PostgresConfiguration)).Get<PostgresConfiguration>();

GoogleCredential? googleCredentials = await GoogleCredential.FromFileAsync("client_secrets.json", default);

InitServiceCollection(builder);
await InitWebApplication(builder);

void InitServiceCollection(WebApplicationBuilder webApplicationBuilder)
{
    webApplicationBuilder.Services.AddControllers(x => x.Filters.Add<AuthenticationFilter>());
    webApplicationBuilder.Services.AddEndpointsApiExplorer();
    webApplicationBuilder.Services.AddSwaggerGen(c =>
    {
        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description = @"JWT Authorization header using the Bearer scheme. \r\n\r\n 
                      Enter 'Bearer' [space] and then your token in the text input below.
                      \r\n\r\nExample: 'Bearer 12345abcdef'",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer",
        });

        c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer",
                    },
                    Scheme = "oauth2",
                    Name = "Bearer",
                    In = ParameterLocation.Header,
                },
                new List<string>()
            }
        });
    });
    webApplicationBuilder.Services.AddApplicationConfiguration();

    webApplicationBuilder.Services
        .AddHandlers()
        .AddApplicationCommands()
        .AddMappingConfiguration();

    webApplicationBuilder.Services
        .AddDatabaseContext(o => o
            .UseNpgsql(postgresConfiguration.ToConnectionString())
            .UseLazyLoadingProxies());

    webApplicationBuilder.Services.AddIdentityConfiguration(webApplicationBuilder.Configuration.GetSection("Identity"),
        x => x.UseSqlite("Filename=shreks-identity.db"));

    if (!googleIntegrationConfiguration.EnableGoogleIntegration)
    {
        webApplicationBuilder.Services
            .AddDummyGoogleIntegration();
    }
    else
    {
        webApplicationBuilder.Services
            .AddGoogleIntegration(o => o
                .ConfigureGoogleCredentials(googleCredentials)
                .ConfigureDriveId(googleIntegrationConfiguration.GoogleDriveId));
    }

    webApplicationBuilder.Services.AddGithubServices(cacheConfiguration, githubIntegrationConfiguration);

    if (webApplicationBuilder.Environment.IsDevelopment())
    {
        webApplicationBuilder.Services
            .AddEntityGenerators(o =>
            {
                o.ConfigureFaker(o => o.Locale = "ru");
                o.ConfigureEntityGenerator<User>(o => o.Count = testEnvironmentConfiguration.Users.Count);
                o.ConfigureEntityGenerator<Student>(o => o.Count = testEnvironmentConfiguration.Users.Count);
                o.ConfigureEntityGenerator<Mentor>(o => o.Count = testEnvironmentConfiguration.Users.Count);
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

    app.UseGithubIntegration(githubIntegrationConfiguration);

    using (var scope = app.Services.CreateScope())
    {
        if (app.Environment.IsDevelopment())
            await InitTestEnvironment(scope.ServiceProvider, testEnvironmentConfiguration);

        await SeedAdmins(scope.ServiceProvider, app.Configuration);
    }

    app.Run();
}

async Task InitTestEnvironment(
    IServiceProvider serviceProvider,
    TestEnvironmentConfiguration config,
    CancellationToken cancellationToken = default)
{
    var dbContext = serviceProvider.GetRequiredService<IShreksDatabaseContext>();

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
    dbContext.SubjectCourseAssociations.Add(
        new GithubSubjectCourseAssociation(subjectCourse, config.Organization, config.TemplateRepository));

    await dbContext.SaveChangesAsync(cancellationToken);
}

async Task SeedAdmins(IServiceProvider provider, IConfiguration configuration)
{
    var mediatr = provider.GetRequiredService<IMediator>();
    var logger = provider.GetRequiredService<ILogger<Program>>();
    var adminsSection = configuration.GetSection("Identity:DefaultAdmins");
    AdminModel[] admins = adminsSection.Get<AdminModel[]>();

    foreach (var admin in admins)
    {
        try
        {
            var registerCommand = new Register.Command(admin.Username, admin.Password);
            await mediatr.Send(registerCommand);

            var promoteCommand = new PromoteToAdmin.Command(admin.Username);
            await mediatr.Send(promoteCommand);
        }
        catch (RegistrationFailedException e)
        {
            logger.LogWarning(e, "Failed registration of {AdminUsername}", admin.Username);
        }
    }
}