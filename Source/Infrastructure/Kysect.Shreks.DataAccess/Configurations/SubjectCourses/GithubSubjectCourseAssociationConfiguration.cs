using Kysect.Shreks.Core.SubjectCourseAssociations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kysect.Shreks.DataAccess.Configurations.SubjectCourses;

public class GithubSubjectCourseAssociationConfiguration : IEntityTypeConfiguration<GithubSubjectCourseAssociation>
{
    public void Configure(EntityTypeBuilder<GithubSubjectCourseAssociation> builder)
    {
        builder.HasIndex(x => x.GithubOrganizationName).IsUnique();
    }
}