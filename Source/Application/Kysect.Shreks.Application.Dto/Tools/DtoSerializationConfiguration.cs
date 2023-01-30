using FluentSerialization;
using Kysect.Shreks.Application.Dto.SubjectCourseAssociations;

namespace Kysect.Shreks.Application.Dto.Tools;

public class DtoSerializationConfiguration : ISerializationConfiguration
{
    public void Configure(ISerializationConfigurationBuilder configurationBuilder)
    {
        configurationBuilder
            .Type<GithubSubjectCourseAssociationDto>()
            .HasTypeKey("GithubAssociation");
    }
}