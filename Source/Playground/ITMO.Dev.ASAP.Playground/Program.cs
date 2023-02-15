using ITMO.Dev.ASAP.DataAccess.Context;
using ITMO.Dev.ASAP.Playground;
using Microsoft.EntityFrameworkCore;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Verbose()
    .WriteTo.Console()
    .WriteTo.File("ShreksPlayground.log")
    .CreateLogger();

DbContextOptionsBuilder<DatabaseContext> builder = new DbContextOptionsBuilder<DatabaseContext>()
    .UseNpgsql("Host=localhost;Port=5433;Database=shreks-dev;Username=postgres;Password=postgres");

var context = new DatabaseContext(builder.Options);
await context.Database.EnsureCreatedAsync();

var subjectDeleter = new SubjectDeleter(context);

await subjectDeleter.DeleteSubject(Guid.Parse("3a7f4e90-ae53-4695-b257-6fb69e98631f"));