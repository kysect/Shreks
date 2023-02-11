using ITMO.Dev.ASAP.Application.Dto.Study;
using ITMO.Dev.ASAP.Application.Dto.SubjectCourseAssociations;

namespace ITMO.Dev.ASAP.Application.Dto.SubjectCourses;

public record SubjectCourseDto(
    Guid Id,
    Guid SubjectId,
    string Title,
    SubmissionStateWorkflowTypeDto? WorkflowType,
    IReadOnlyCollection<SubjectCourseAssociationDto> Associations);