using ITMO.Dev.ASAP.DataAccess.Context;
using Microsoft.EntityFrameworkCore;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Verbose()
    .WriteTo.Console()
    .WriteTo.File("ShreksPlayground.log")
    .CreateLogger();

Console.WriteLine("Hello, World!");

DbContextOptionsBuilder<DatabaseContext> builder = new DbContextOptionsBuilder<DatabaseContext>()
    .UseSqlite("Data Source=ShreksPlayground.db");

var context = new DatabaseContext(builder.Options);
await context.Database.EnsureCreatedAsync();