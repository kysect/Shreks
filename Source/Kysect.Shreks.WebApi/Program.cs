using Kysect.Shreks.Application.Commands.Extensions;
using Kysect.Shreks.Application.Handlers.Extensions;
using Kysect.Shreks.DataAccess.Extensions;
using Kysect.Shreks.Integration.Github.Extensions;
using Kysect.Shreks.Integration.Github.Helpers;
using Kysect.Shreks.Integration.Google.Extensions;
using Kysect.Shreks.Mapping.Extensions;
using Kysect.Shreks.Seeding.Extensions;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog(
    (ctx, lc) => lc.MinimumLevel.Verbose().WriteTo.Console());

ShreksConfiguration shreksConfiguration = builder.Configuration.GetShreksConfiguration();

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

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();
    app.UseSerilogRequestLogging();

    app.UseGithubIntegration(shreksConfiguration);

    app.Run();
}