using Kysect.Shreks.Core.SubjectCourseAssociations;
using Kysect.Shreks.DataAccess.Abstractions;
using Kysect.Shreks.Mapping.Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static Kysect.Shreks.Application.Contracts.Github.Commands.RemoveGithubSubjectCourseAssociation;

namespace Kysect.Shreks.Application.Handlers.Github;

internal class RemoveGithubSubjectCourseAssociationHandler : IRequestHandler<Command, Response>
{
    private readonly IShreksDatabaseContext _context;

    public RemoveGithubSubjectCourseAssociationHandler(IShreksDatabaseContext context)
    {
        _context = context;
    }

    public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
    {
        GithubSubjectCourseAssociation subjectCourseAssociation = await _context
            .SubjectCourseAssociations
            .Include(x => x.SubjectCourse)
            .Where(gsa => gsa.SubjectCourse.Id == request.SubjectCourseId)
            .OfType<GithubSubjectCourseAssociation>()
            .FirstAsync(cancellationToken);

        _context.SubjectCourseAssociations.Remove(subjectCourseAssociation);
        await _context.SaveChangesAsync(cancellationToken);

        return new Response(subjectCourseAssociation.SubjectCourse.ToDto());
    }
}