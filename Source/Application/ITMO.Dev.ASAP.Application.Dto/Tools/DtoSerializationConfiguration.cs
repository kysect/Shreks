using FluentSerialization;
using ITMO.Dev.ASAP.Application.Dto.SubjectCourseAssociations;

namespace ITMO.Dev.ASAP.Application.Dto.Tools;

public class DtoSerializationConfiguration : ISerializationConfiguration
{
    public void Configure(ISerializationConfigurationBuilder configurationBuilder)
    {
        configurationBuilder
            .Type<GithubSubjectCourseAssociationDto>()
            .HasTypeKey("GithubSubjectCourseAssociation");

        configurationBuilder
            .Type<GoogleSubjectCourseAssociationDto>()
            .HasTypeKey("GoogleSubjectCourseAssociation");
    }
}