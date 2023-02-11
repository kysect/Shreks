using ITMO.Dev.ASAP.Application.Dto.Study;
using ITMO.Dev.ASAP.Application.Dto.SubjectCourseAssociations;

namespace ITMO.Dev.ASAP.WebApi.Abstractions.Models.SubjectCourses;

public record CreateSubjectCourseRequest(
    Guid SubjectId,
    string Name,
    SubmissionStateWorkflowTypeDto WorkflowType,
    IReadOnlyCollection<SubjectCourseAssociationDto> Associations);