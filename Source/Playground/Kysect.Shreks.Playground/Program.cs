using Kysect.Shreks.Core.Queue;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Core.Users;
using Kysect.Shreks.DataAccess.Context;
using Microsoft.EntityFrameworkCore;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Verbose()
    .WriteTo.Console()
    .WriteTo.File("ShreksPlayground.log")
    .CreateLogger();

Console.WriteLine("Hello, World!");

var builder = new DbContextOptionsBuilder<ShreksDatabaseContext>()
    .UseSqlite("Data Source=ShreksPlayground.db");

var context = new ShreksDatabaseContext(builder.Options);
await context.Database.EnsureCreatedAsync();


var submission = new Submission
(
    new Student(new User("John", "Doe", "adw"),
        new StudentGroup("")),
    new Assignment("adw", "", 1, 2, new SubjectCourse(new Subject(""))),
    DateOnly.FromDateTime(DateTime.Now),
    ""
);

var positionedSubmission = new PositionedSubmission
(
    1,
    submission
);

context.Add(positionedSubmission);

await context.SaveChangesAsync();

context.Remove(positionedSubmission);
await context.SaveChangesAsync();