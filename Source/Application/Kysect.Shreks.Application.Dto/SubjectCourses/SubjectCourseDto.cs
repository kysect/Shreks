using Kysect.Shreks.Application.Dto.Study;

namespace Kysect.Shreks.Application.Dto.SubjectCourses;

public record SubjectCourseDto(
    Guid Id,
    Guid SubjectId,
    string Title,
    SubmissionStateWorkflowTypeDto? WorkflowType,
    IReadOnlyCollection<SubjectCourseAssociationDto> Associations);