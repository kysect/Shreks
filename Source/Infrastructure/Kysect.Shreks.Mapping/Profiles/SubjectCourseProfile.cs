using AutoMapper;
using Kysect.Shreks.Application.Dto.SubjectCourses;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Core.SubjectCourseAssociations;

namespace Kysect.Shreks.Mapping.Profiles;

internal class SubjectCourseProfile : Profile
{
    public SubjectCourseProfile()
    {
        CreateMap<SubjectCourse, SubjectCourseDto>();

        CreateMap<GoogleTableSubjectCourseAssociation, SubjectCourseAssociationDto>().ConstructUsing(x =>
            new SubjectCourseAssociationDto(nameof(GoogleTableSubjectCourseAssociation), x.SpreadsheetId));

        CreateMap<GithubSubjectCourseAssociation, SubjectCourseAssociationDto>().ConstructUsing(x =>
            new SubjectCourseAssociationDto(nameof(GithubSubjectCourseAssociation), x.GithubOrganizationName));
    }
}