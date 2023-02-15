using ITMO.Dev.ASAP.Core.Study;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ITMO.Dev.ASAP.DataAccess.Configurations.SubjectCourses;

public class SubjectCourseConfiguration : IEntityTypeConfiguration<SubjectCourse>
{
    public void Configure(EntityTypeBuilder<SubjectCourse> builder)
    {
        builder.HasOne(x => x.Subject);

        builder.Navigation(x => x.Groups).HasField("_groups");
        builder.Navigation(x => x.Assignments).HasField("_assignments");
        builder.Navigation(x => x.Associations).HasField("_associations");
        builder.Navigation(x => x.Mentors).HasField("_mentors");
        builder.Navigation(x => x.DeadlinePolicies).HasField("_deadlinePolicies");
    }
}