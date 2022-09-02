using AutoMapper;
using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Core.Submissions;

namespace Kysect.Shreks.Mapping.Profiles;

public class StudyProfile : Profile
{
    public StudyProfile()
    {
        CreateMap<Submission, SubmissionDto>();
        CreateMap<SubjectCourseGroup, SubjectCourseGroupDto>();
        CreateMap<Assignment, AssignmentDto>()
            .ForCtorParam("SubjectCourseId", opt =>
                    opt.MapFrom(src => src.SubjectCourse.Id));
    }
}