using AutoMapper;
using Kysect.Shreks.Application.Dto.SubjectCourses;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Core.SubmissionStateWorkflows;
using Kysect.Shreks.DataAccess.Abstractions;
using Kysect.Shreks.DataAccess.Abstractions.Extensions;
using MediatR;
using static Kysect.Shreks.Application.Contracts.Study.Commands.CreateSubjectCourse;

namespace Kysect.Shreks.Application.Handlers.SubjectCourses;

internal class CreateSubjectCourseHandler : IRequestHandler<Command, Response>
{
    private readonly IShreksDatabaseContext _context;
    private readonly IMapper _mapper;

    public CreateSubjectCourseHandler(IShreksDatabaseContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
    {
        Subject subject = await _context.Subjects.GetByIdAsync(request.SubjectId, cancellationToken);
        SubmissionStateWorkflowType workflowType = _mapper.Map<SubmissionStateWorkflowType>(request.WorkflowType);

        var subjectCourse = new SubjectCourse(
            Guid.NewGuid(),
            subject,
            request.Title,
            workflowType);

        _context.SubjectCourses.Add(subjectCourse);
        await _context.SaveChangesAsync(cancellationToken);

        return new Response(_mapper.Map<SubjectCourseDto>(subjectCourse));
    }
}