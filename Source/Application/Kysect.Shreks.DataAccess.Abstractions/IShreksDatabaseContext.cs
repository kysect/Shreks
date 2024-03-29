using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Core.SubjectCourseAssociations;
using Kysect.Shreks.Core.SubmissionAssociations;
using Kysect.Shreks.Core.Submissions;
using Kysect.Shreks.Core.UserAssociations;
using Kysect.Shreks.Core.Users;
using Microsoft.EntityFrameworkCore;

namespace Kysect.Shreks.DataAccess.Abstractions;

public interface IShreksDatabaseContext
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

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}