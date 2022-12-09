using AutoMapper;
using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Application.Dto.SubjectCourses;
using Kysect.Shreks.Core.Study;

namespace Kysect.Shreks.Mapping.Profiles;

public class StudyProfile : Profile
{
    public StudyProfile()
    {
        CreateMap<GroupAssignment, GroupAssignmentDto>();
        CreateMap<SubjectCourseGroup, SubjectCourseGroupDto>();
        CreateMap<Assignment, AssignmentDto>()
            .ForCtorParam(nameof(AssignmentDto.SubjectCourseId), opt =>
                opt.MapFrom(src => src.SubjectCourse.Id));

        CreateMap<StudentGroup, StudyGroupDto>();
        CreateMap<Subject, SubjectDto>();
        CreateMap<SubjectCourse, SubjectCourseDto>();
    }
}