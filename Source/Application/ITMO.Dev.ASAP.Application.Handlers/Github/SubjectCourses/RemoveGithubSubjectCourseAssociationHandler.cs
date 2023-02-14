using ITMO.Dev.ASAP.Core.SubjectCourseAssociations;
using ITMO.Dev.ASAP.DataAccess.Abstractions;
using ITMO.Dev.ASAP.Mapping.Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static ITMO.Dev.ASAP.Application.Contracts.Github.Commands.RemoveGithubSubjectCourseAssociation;

namespace ITMO.Dev.ASAP.Application.Handlers.Github.SubjectCourses;

internal class RemoveGithubSubjectCourseAssociationHandler : IRequestHandler<Command, Response>
{
    private readonly IDatabaseContext _context;

    public RemoveGithubSubjectCourseAssociationHandler(IDatabaseContext context)
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