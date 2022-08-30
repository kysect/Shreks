using Kysect.Shreks.Core.Queue.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kysect.Shreks.DataAccess.Configurations.Queue.Filters;

public class SubjectCourseFilterConfiguration : IEntityTypeConfiguration<SubjectCoursesFilter>
{
    public void Configure(EntityTypeBuilder<SubjectCoursesFilter> builder)
    {
        //builder.HasMany(x => x.Courses).WithMany("Filters");
    }
}