using ITMO.Dev.ASAP.Core.DeadlinePolicies;
using ITMO.Dev.ASAP.Core.Study;
using ITMO.Dev.ASAP.Core.SubjectCourseAssociations;
using ITMO.Dev.ASAP.Core.SubmissionAssociations;
using ITMO.Dev.ASAP.Core.Submissions;
using ITMO.Dev.ASAP.Core.UserAssociations;
using ITMO.Dev.ASAP.Core.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace ITMO.Dev.ASAP.DataAccess.Abstractions;

public interface IDatabaseContext
{
    DbSet<User> Users { get; }

    DbSet<Student> Students { get; }

    DbSet<Mentor> Mentors { get; }

    DbSet<Assignment> Assignments { get; }

    DbSet<GroupAssignment> GroupAssignments { get; }

    DbSet<StudentGroup> StudentGroups { get; }

    DbSet<Subject> Subjects { get; }

    DbSet<SubjectCourse> SubjectCourses { get; }

    DbSet<SubjectCourseGroup> SubjectCourseGroups { get; }

    DbSet<Submission> Submissions { get; }

    DbSet<SubmissionAssociation> SubmissionAssociations { get; }

    DbSet<UserAssociation> UserAssociations { get; }

    DbSet<SubjectCourseAssociation> SubjectCourseAssociations { get; }

    DbSet<DeadlinePolicy> DeadlinePolicies { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);

    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken);
}