using Kysect.Shreks.Core.DeadlinePolicies;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Core.ValueObject;
using Kysect.Shreks.DataAccess.Abstractions;
using Kysect.Shreks.DataAccess.Abstractions.Extensions;
using MediatR;
using static Kysect.Shreks.Application.Abstractions.Study.Commands.AddFractionDeadlinePolicy;

namespace Kysect.Shreks.Application.Handlers.Study;

public class AddFractionDeadlinePolicyHandling : IRequestHandler<Command, Response>
{
    private readonly IShreksDatabaseContext _context;

    public AddFractionDeadlinePolicyHandling(IShreksDatabaseContext context)
    {
        _context = context;
    }

    public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
    {
        SubjectCourse subjectCourse = await _context.SubjectCourses.GetByIdAsync(request.SubjectCourseId, cancellationToken);
        subjectCourse.AddDeadlinePolicy(new FractionDeadlinePolicy(request.SpanBeforeActivation, new Fraction(request.Fraction)));
        await _context.SaveChangesAsync(cancellationToken);
        return new Response();
    }
}