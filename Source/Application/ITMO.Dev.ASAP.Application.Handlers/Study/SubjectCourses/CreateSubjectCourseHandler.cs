using ITMO.Dev.ASAP.Core.Study;
using ITMO.Dev.ASAP.Core.SubjectCourseAssociations;
using ITMO.Dev.ASAP.Core.SubmissionStateWorkflows;
using ITMO.Dev.ASAP.DataAccess.Abstractions;
using ITMO.Dev.ASAP.DataAccess.Abstractions.Extensions;
using ITMO.Dev.ASAP.Mapping.Mappings;
using MediatR;
using static ITMO.Dev.ASAP.Application.Contracts.Study.SubjectCourses.Commands.CreateSubjectCourse;

namespace ITMO.Dev.ASAP.Application.Handlers.Study.SubjectCourses;

internal class CreateSubjectCourseHandler : IRequestHandler<Command, Response>
{
    private readonly IDatabaseContext _context;

    public CreateSubjectCourseHandler(IDatabaseContext context)
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