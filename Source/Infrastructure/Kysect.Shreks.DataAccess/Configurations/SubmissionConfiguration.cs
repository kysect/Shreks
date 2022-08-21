using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Submission = Kysect.Shreks.Core.Submissions.Submission;

namespace Kysect.Shreks.DataAccess.Configurations;

public class SubmissionConfiguration : IEntityTypeConfiguration<Submission>
{
    public void Configure(EntityTypeBuilder<Submission> builder)
    {
        builder.HasOne(x => x.Student);
        builder.HasOne(x => x.GroupAssignment);
        builder.Navigation(x => x.Associations).HasField("_associations");
    }
}