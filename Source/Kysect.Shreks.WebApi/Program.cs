using Kysect.Shreks.Application.Handlers.Extensions;
using Kysect.Shreks.DataAccess.Extensions;
using Kysect.Shreks.Mapping.Extensions;
using Kysect.Shreks.Seeding.Extensions;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog(
    (ctx, lc) => lc.MinimumLevel.Verbose().WriteTo.Console());

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHandlers();
builder.Services.AddDatabaseContext(
    opt => opt.UseInMemoryDatabase("ShreksDb").UseLazyLoadingProxies());
builder.Services.AddMappingConfiguration();

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddDatabaseSeeders();
}

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors(opt => opt.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.UseSerilogRequestLogging();

app.Run();