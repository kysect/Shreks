using ITMO.Dev.ASAP.Core.DeadlinePolicies;
using ITMO.Dev.ASAP.Core.Study;
using ITMO.Dev.ASAP.DataAccess.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static ITMO.Dev.ASAP.Application.Contracts.Study.SubjectCourses.Commands.AddFractionDeadlinePolicy;

namespace ITMO.Dev.ASAP.Application.Handlers.Study.SubjectCourses;

internal class AddFractionDeadlinePolicyHandling : IRequestHandler<Command>
{
    private readonly IDatabaseContext _context;

    public AddFractionDeadlinePolicyHandling(IDatabaseContext context)
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