using Google.Apis.Auth.OAuth2;
using Kysect.Shreks.Application.Commands.Extensions;
using Kysect.Shreks.Application.Contracts.Identity.Commands;
using Kysect.Shreks.Application.Extensions;
using Kysect.Shreks.Application.GithubWorkflow.Abstractions;
using Kysect.Shreks.Application.GithubWorkflow.Extensions;
using Kysect.Shreks.Application.Google.Extensions;
using Kysect.Shreks.Application.Handlers.Extensions;
using Kysect.Shreks.Application.Services.Extensions;
using Kysect.Shreks.Common.Exceptions;
using Kysect.Shreks.Controllers;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Core.SubjectCourseAssociations;
using Kysect.Shreks.Core.Submissions;
using Kysect.Shreks.Core.UserAssociations;
using Kysect.Shreks.Core.Users;
using Kysect.Shreks.DataAccess.Configuration;
using Kysect.Shreks.DataAccess.Extensions;
using Kysect.Shreks.DeveloperEnvironment;
using Kysect.Shreks.Identity.Extensions;
using Kysect.Shreks.Integration.Github.Extensions;
using Kysect.Shreks.Integration.Github.Helpers;
using Kysect.Shreks.Integration.Google.Models;
using Kysect.Shreks.Mapping.Extensions;
using Kysect.Shreks.Seeding.Extensions;
using Kysect.Shreks.WebApi.Abstractions.Models;
using Kysect.Shreks.WebApi.Extensions;
using Kysect.Shreks.WebApi.Filters;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilogForAppLogs(builder.Configuration);

var googleIntegrationConfiguration = builder.Configuration.GetSection(nameof(GoogleIntegrationConfiguration))
    .Get<GoogleIntegrationConfiguration>();
var cacheConfiguration = builder.Configuration.GetSection(nameof(CacheConfiguration)).Get<CacheConfiguration>();
var githubIntegrationConfiguration = builder.Configuration.GetSection(nameof(GithubIntegrationConfiguration))
    .Get<GithubIntegrationConfiguration>();
var testEnvironmentConfiguration = builder.Configuration.GetSection(nameof(TestEnvironmentConfiguration))
    .Get<TestEnvironmentConfiguration>();
var postgresConfiguration = builder.Configuration.GetSection(nameof(PostgresConfiguration)).Get<PostgresConfiguration>();
var dbNames = builder.Configuration.GetSection(nameof(DbNamesConfiguration)).Get<DbNamesConfiguration>();

InitServiceCollection(builder);
await InitWebApplication(builder);

void InitServiceCollection(WebApplicationBuilder webApplicationBuilder)
{
    if (testEnvironmentConfiguration is not null)
    {
        webApplicationBuilder.Services.TryAddSingleton(testEnvironmentConfiguration);
    }

    webApplicationBuilder.Services
        .AddControllers(x => x.Filters.Add<AuthenticationFilter>())
        .AddNewtonsoftJson()
        .AddApplicationPart(typeof(IControllersProjectMarker).Assembly)
        .AddControllersAsServices();

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
                    Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer", },
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
        .AddMappingConfiguration()
        .AddApplicationServices();

    webApplicationBuilder.Services
        .AddDatabaseContext(o => o
            .UseNpgsql(postgresConfiguration.ToConnectionString(dbNames.ApplicationDbName))
            .UseLazyLoadingProxies());

    webApplicationBuilder.Services.AddIdentityConfiguration(
        webApplicationBuilder.Configuration.GetSection("Identity").GetSection("IdentityConfiguration"),
        x => x.UseNpgsql(postgresConfiguration.ToConnectionString(dbNames.IdentityDbName)));

    if (!googleIntegrationConfiguration.EnableGoogleIntegration)
    {
        webApplicationBuilder.Services
            .AddDummyGoogleIntegration();
    }
    else
    {
        webApplicationBuilder.Services
            .AddGoogleIntegration(o => o
                .ConfigureGoogleCredentials(GoogleCredential.FromJson(googleIntegrationConfiguration.ClientSecrets))
                .ConfigureDriveId(googleIntegrationConfiguration.GoogleDriveId));
    }

    webApplicationBuilder.Services
        .AddGithubServices(cacheConfiguration, githubIntegrationConfiguration)
        .AddGithubWorkflowServices();

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
            .AddDatabaseSeeders()
            .AddDeveloperEnvironmentSeeding();
    }

    webApplicationBuilder.Services.AddRazorPages();
}

async Task InitWebApplication(WebApplicationBuilder webApplicationBuilder)
{
    var app = webApplicationBuilder.Build();

    app.UseRequestLogging();

    if (app.Environment.IsDevelopment())
    {
        //await app.Services.UseDatabaseSeeders();
        app.UseWebAssemblyDebugging();
    }

    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors(o => o.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

    app
        .UseBlazorFrameworkFiles()
        .UseStaticFiles()
        .UseRouting();

    app.MapRazorPages();

    app
        .UseAuthentication()
        .UseAuthorization();
    
    app.MapFallbackToFile("index.html");

    app.MapControllers();
    app.UseGithubIntegration(githubIntegrationConfiguration);

    using (var scope = app.Services.CreateScope())
    {
        await SeedAdmins(scope.ServiceProvider, app.Configuration);
        await scope.ServiceProvider.UseDatabaseContext();
    }

    app.Run();
}

async Task SeedAdmins(IServiceProvider provider, IConfiguration configuration)
{
    var mediatr = provider.GetRequiredService<IMediator>();
    var logger = provider.GetRequiredService<ILogger<Program>>();
    var adminsSection = configuration.GetSection("Identity:DefaultAdmins");
    AdminModel[] admins = adminsSection.Get<AdminModel[]>() ?? Array.Empty<AdminModel>();

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