using AutoMapper;
using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Core.Study;

namespace Kysect.Shreks.Mapping.Profiles;

public class SubmissionDtoProfile : Profile
{
    public SubmissionDtoProfile()
    {
        CreateMap<Submission, SubmissionDto>()
            .ForCtorParam(
                nameof(SubmissionDto.Points),
                opt => opt.MapFrom(s => s.Points.Value))
            .ForCtorParam(
                nameof(SubmissionDto.ExtraPoints),
                opt => opt.MapFrom(s => s.ExtraPoints.Value));
    }
}