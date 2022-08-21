using AutoMapper;
using Kysect.Shreks.Application.Dto.Study;
using Submission = Kysect.Shreks.Core.Submissions.Submission;

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
    }
}