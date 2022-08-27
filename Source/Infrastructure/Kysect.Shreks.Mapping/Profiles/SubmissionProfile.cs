using AutoMapper;
using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Application.Dto.Tables;
using Kysect.Shreks.Core.Submissions;

namespace Kysect.Shreks.Mapping.Profiles;

public class SubmissionProfile : Profile
{
    public SubmissionProfile()
    {
        CreateMap<Submission, SubmissionDto>()
            .ForCtorParam(
                nameof(SubmissionDto.AssignmentId),
                opt => opt.MapFrom(src => src.GroupAssignment.AssignmentId))
            .ForCtorParam(
                nameof(SubmissionDto.AssignmentShortName),
                opt => opt.MapFrom(src => src.GroupAssignment.Assignment.ShortName));

        CreateMap<Submission, QueueSubmissionDto>()
            .ForCtorParam(nameof(QueueSubmissionDto.Submission), 
                opt => opt.MapFrom(x => x))
            .ForCtorParam(nameof(QueueSubmissionDto.Student), 
                opt => opt.MapFrom(x => x.Student));
    }
}