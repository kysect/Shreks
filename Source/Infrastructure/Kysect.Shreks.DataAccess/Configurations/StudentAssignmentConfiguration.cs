using Kysect.Shreks.Core.Study;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kysect.Shreks.DataAccess.Configurations;

public class StudentAssignmentConfiguration : IEntityTypeConfiguration<StudentAssignment>
{
    public void Configure(EntityTypeBuilder<StudentAssignment> builder)
    {
        builder.HasKey(x => new { x.StudentId, x.AssignmentId });

        builder.HasOne(x => x.Student);
        builder.HasOne(x => x.Assignment);
        
        builder.Navigation(x => x.Submissions).HasField("_submissions");
    }
}