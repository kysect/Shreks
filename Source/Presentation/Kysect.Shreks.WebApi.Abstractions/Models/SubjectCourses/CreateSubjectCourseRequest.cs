using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Application.Dto.SubjectCourseAssociations;

namespace Kysect.Shreks.WebApi.Abstractions.Models.SubjectCourses;

public record CreateSubjectCourseRequest(
    Guid SubjectId,
    string Name,
    SubmissionStateWorkflowTypeDto WorkflowType,
    IReadOnlyCollection<SubjectCourseAssociationDto> Associations);