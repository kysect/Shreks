using Kysect.Shreks.Core.Queue;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Core.Submissions;
using Kysect.Shreks.Core.Users;
using Kysect.Shreks.Core.ValueObject;
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


var assignment = new Assignment("adw", "", new Points(1), new Points(2), new SubjectCourse(new Subject(""), ""));
var submission = new GithubSubmission
(
    new Student(new User("John", "Doe", "adw"),
        new StudentGroup("")),
    new GroupAssignment(new StudentGroup(""), assignment, DateOnly.FromDateTime(DateTime.Now)),
    DateOnly.FromDateTime(DateTime.Now),
    "",
    "",
    "",
    0
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