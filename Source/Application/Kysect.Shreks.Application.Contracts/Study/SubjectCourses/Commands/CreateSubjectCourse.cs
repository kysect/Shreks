using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Application.Dto.SubjectCourses;
using MediatR;
using SubjectCourseAssociationDto = Kysect.Shreks.Application.Dto.SubjectCourseAssociations.SubjectCourseAssociationDto;

namespace Kysect.Shreks.Application.Contracts.Study.SubjectCourses.Commands;

internal static class CreateSubjectCourse
{
    public record Command(
        Guid SubjectId,
        string Title,
        SubmissionStateWorkflowTypeDto WorkflowType,
        IReadOnlyCollection<SubjectCourseAssociationDto> Associations) : IRequest<Response>;

    public record Response(SubjectCourseDto SubjectCourse);
}