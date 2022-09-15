namespace Kysect.Shreks.Application.Dto.Study;

public record SubjectCourseDto(
    Guid Id,
    Guid SubjectId,
    string Title,
    SubmissionStateWorkflowTypeDto? WorkflowType,
    IReadOnlyCollection<SubjectCourseAssociationDto> Associations);