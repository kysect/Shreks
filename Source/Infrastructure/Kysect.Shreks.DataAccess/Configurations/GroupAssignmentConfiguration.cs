using Kysect.Shreks.Core.Study;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kysect.Shreks.DataAccess.Configurations;

public class GroupAssignmentConfiguration : IEntityTypeConfiguration<GroupAssignment>
{
    public void Configure(EntityTypeBuilder<GroupAssignment> builder)
    {
        builder.HasKey(x => new { x.GroupId, x.AssignmentId });
    }
}