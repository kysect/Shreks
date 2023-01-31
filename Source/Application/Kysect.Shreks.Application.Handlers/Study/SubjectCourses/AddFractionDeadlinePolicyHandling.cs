using Kysect.Shreks.Core.DeadlinePolicies;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.DataAccess.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static Kysect.Shreks.Application.Contracts.Study.Commands.AddFractionDeadlinePolicy;

namespace Kysect.Shreks.Application.Handlers.Study.SubjectCourses;

internal class AddFractionDeadlinePolicyHandling : IRequestHandler<Command>
{
    private readonly IShreksDatabaseContext _context;

    public AddFractionDeadlinePolicyHandling(IShreksDatabaseContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
    {
        SubjectCourse subjectCourse = await _context.SubjectCourses
            .Include(x => x.DeadlinePolicies)
            .SingleAsync(x => x.Id.Equals(request.SubjectCourseId), cancellationToken);

        var deadlinePolicy = new FractionDeadlinePolicy(request.SpanBeforeActivation, request.Fraction);
        subjectCourse.AddDeadlinePolicy(deadlinePolicy);

        _context.SubjectCourses.Update(subjectCourse);
        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}