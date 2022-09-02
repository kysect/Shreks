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
        CreateMap<GroupAssignment, GroupAssignmentDto>();
        CreateMap<SubjectCourseGroup, SubjectCourseGroupDto>();
        CreateMap<Assignment, AssignmentDto>()
            .ForCtorParam("SubjectCourseId", opt =>
                    opt.MapFrom(src => src.SubjectCourse.Id));
    }
}