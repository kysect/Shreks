using Kysect.Shreks.DataAccess.Context;
using Microsoft.EntityFrameworkCore;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Verbose()
    .WriteTo.Console()
    .WriteTo.File("ShreksPlayground.log")
    .CreateLogger();

Console.WriteLine("Hello, World!");

DbContextOptionsBuilder<ShreksDatabaseContext> builder = new DbContextOptionsBuilder<ShreksDatabaseContext>()
    .UseSqlite("Data Source=ShreksPlayground.db");

var context = new ShreksDatabaseContext(builder.Options);
await context.Database.EnsureCreatedAsync();