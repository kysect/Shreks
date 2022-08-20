using Kysect.Shreks.Core.Queue;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Core.SubjectCourseAssociations;
using Kysect.Shreks.Core.SubmissionAssociations;
using Kysect.Shreks.Core.UserAssociations;
using Kysect.Shreks.Core.Users;
using Kysect.Shreks.Core.ValueObject;
using Kysect.Shreks.DataAccess.Abstractions;
using Kysect.Shreks.DataAccess.ValueConverters;
using Microsoft.EntityFrameworkCore;

namespace Kysect.Shreks.DataAccess.Context;

public class ShreksDatabaseContext : DbContext, IShreksDatabaseContext
{
    public ShreksDatabaseContext(DbContextOptions<ShreksDatabaseContext> options) : base(options) { }

    public DbSet<User> Users { get; protected init; } = null!;
    public DbSet<Student> Students { get; protected init; } = null!;
    public DbSet<Mentor> Mentors { get; protected init; } = null!;
    public DbSet<Assignment> Assignments { get; protected init; } = null!;
    public DbSet<GroupAssignment> GroupAssignments { get; protected init; } = null!;
    public DbSet<StudentGroup> StudentGroups { get; protected init; } = null!;
    public DbSet<Subject> Subjects { get; protected init; } = null!;
    public DbSet<SubjectCourse> SubjectCourses { get; protected init; } = null!;
    public DbSet<SubjectCourseGroup> SubjectCourseGroups { get; protected init; } = null!;
    public DbSet<Submission> Submissions { get; protected init; } = null!;
    public DbSet<SubmissionAssociation> SubmissionAssociations { get; protected init; } = null!;
    public DbSet<UserAssociation> UserAssociations { get; protected init; } = null!;
    public DbSet<SubjectCourseAssociation> SubjectCourseAssociations { get; protected init; } = null!;
    public DbSet<SubmissionQueue> SubmissionQueues { get; protected init; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(IAssemblyMarker).Assembly);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<Points>().HaveConversion<PointsValueConverter>();
        configurationBuilder.Properties<Fraction>().HaveConversion<FractionValueConverter>();
    }
}