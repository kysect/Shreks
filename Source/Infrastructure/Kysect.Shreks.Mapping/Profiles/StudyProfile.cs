using AutoMapper;
using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Core.Study;

namespace Kysect.Shreks.Mapping.Profiles;

public class StudyProfile : Profile
{
    public StudyProfile()
    {
        CreateMap<Submission, SubmissionDto>();
        CreateMap<Assignment, AssignmentDto>();
    }
}