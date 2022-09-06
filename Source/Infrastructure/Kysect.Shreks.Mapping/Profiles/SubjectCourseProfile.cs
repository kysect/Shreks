using AutoMapper;
using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Core.SubjectCourseAssociations;

namespace Kysect.Shreks.Mapping.Profiles
{
    internal class SubjectCourserProfile : Profile
    {
        public SubjectCourserProfile()
        {
            CreateMap<SubjectCourseAssociation, SubjectCourseAssociationDto>();
        }
    }
}
