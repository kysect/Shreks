using ITMO.Dev.ASAP.Core.SubjectCourseAssociations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ITMO.Dev.ASAP.DataAccess.Configurations.SubjectCourses;

public class GithubSubjectCourseAssociationConfiguration : IEntityTypeConfiguration<GithubSubjectCourseAssociation>
{
    public void Configure(EntityTypeBuilder<GithubSubjectCourseAssociation> builder)
    {
        builder.HasIndex(x => x.GithubOrganizationName).IsUnique();
    }
}