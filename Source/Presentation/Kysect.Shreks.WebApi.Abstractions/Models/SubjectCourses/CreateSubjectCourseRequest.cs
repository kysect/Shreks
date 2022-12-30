using Kysect.Shreks.Application.Dto.Study;

namespace Kysect.Shreks.WebApi.Abstractions.Models.SubjectCourses;

public record CreateSubjectCourseRequest(Guid SubjectId, string Name, SubmissionStateWorkflowTypeDto WorkflowType);