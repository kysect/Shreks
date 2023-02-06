using Kysect.Shreks.Core.Study;
using Kysect.Shreks.DataAccess.Abstractions;
using Kysect.Shreks.DataAccess.Abstractions.Extensions;
using Kysect.Shreks.Mapping.Mappings;
using MediatR;
using static Kysect.Shreks.Application.Contracts.Study.SubjectCourses.Commands.UpdateSubjectCourse;

namespace Kysect.Shreks.Application.Handlers.Study.SubjectCourses;

internal class UpdateSubjectCourseHandler : IRequestHandler<Command, Response>
{
    private readonly IShreksDatabaseContext _context;

    public UpdateSubjectCourseHandler(IShreksDatabaseContext context)
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