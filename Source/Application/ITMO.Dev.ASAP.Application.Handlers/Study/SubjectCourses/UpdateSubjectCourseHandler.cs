using ITMO.Dev.ASAP.Core.Study;
using ITMO.Dev.ASAP.DataAccess.Abstractions;
using ITMO.Dev.ASAP.DataAccess.Abstractions.Extensions;
using ITMO.Dev.ASAP.Mapping.Mappings;
using MediatR;
using static ITMO.Dev.ASAP.Application.Contracts.Study.SubjectCourses.Commands.UpdateSubjectCourse;

namespace ITMO.Dev.ASAP.Application.Handlers.Study.SubjectCourses;

internal class UpdateSubjectCourseHandler : IRequestHandler<Command, Response>
{
    private readonly IDatabaseContext _context;

    public UpdateSubjectCourseHandler(IDatabaseContext context)
    {
        _context = context;
    }

    public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
    {
        SubjectCourse subjectCourse = await _context.SubjectCourses.GetByIdAsync(request.Id, cancellationToken);
        subjectCourse.Title = request.NewTitle;

        _context.SubjectCourses.Update(subjectCourse);
        await _context.SaveChangesAsync(cancellationToken);

        return new Response(subjectCourse.ToDto());
    }
}