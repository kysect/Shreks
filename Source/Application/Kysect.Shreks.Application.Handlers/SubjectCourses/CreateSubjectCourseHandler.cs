using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Core.SubjectCourseAssociations;
using Kysect.Shreks.Core.SubmissionStateWorkflows;
using Kysect.Shreks.DataAccess.Abstractions;
using Kysect.Shreks.DataAccess.Abstractions.Extensions;
using Kysect.Shreks.Mapping.Mappings;
using MediatR;
using static Kysect.Shreks.Application.Contracts.Study.Commands.CreateSubjectCourse;

namespace Kysect.Shreks.Application.Handlers.SubjectCourses;

internal class CreateSubjectCourseHandler : IRequestHandler<Command, Response>
{
    private readonly IShreksDatabaseContext _context;

    public CreateSubjectCourseHandler(IShreksDatabaseContext context)
    {
        _context = context;
    }

    public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
    {
        Subject subject = await _context.Subjects.GetByIdAsync(request.SubjectId, cancellationToken);
        SubmissionStateWorkflowType workflowType = request.WorkflowType.AsValueObject();

        var subjectCourse = new SubjectCourse(
            Guid.NewGuid(),
            subject,
            request.Title,
            workflowType);

        IEnumerable<SubjectCourseAssociation> associations = request.Associations
            .Select(x => x.ToEntity(subjectCourse));

        foreach (SubjectCourseAssociation association in associations)
        {
            subjectCourse.AddAssociation(association);
            _context.SubjectCourseAssociations.Add(association);
        }

        _context.SubjectCourses.Add(subjectCourse);
        await _context.SaveChangesAsync(cancellationToken);

        return new Response(subjectCourse.ToDto());
    }
}