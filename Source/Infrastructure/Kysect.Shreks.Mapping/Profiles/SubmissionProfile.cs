using AutoMapper;
using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Core.Study;

namespace Kysect.Shreks.Mapping.Profiles;

public class SubmissionProfile : Profile
{
    public SubmissionProfile()
    {
        CreateMap<Submission, SubmissionDto>()
            .ConstructUsing(s => new SubmissionDto(
                s.Id,
                s.SubmissionDate,
                s.Student.Id,
                s.GroupAssignment.AssignmentId,
                s.Payload,
                s.ExtraPoints == null ? 0 : s.ExtraPoints.Value.Value,
                s.Points == null ? 0 : s.Points.Value.Value,
                s.GroupAssignment.Assignment.ShortName));
            //.ForMember(dto => dto.AssignmentId,
            //    opt => opt.MapFrom(src => src.GroupAssignment.AssignmentId))
            //.ForMember(dto => dto.AssignmentShortName,
            //    opt => opt.MapFrom(src => src.GroupAssignment.Assignment.ShortName));
    }
}