using Kysect.Shreks.Application.Factories;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Core.SubjectCourseAssociations;
using Kysect.Shreks.DataAccess.Abstractions;
using Kysect.Shreks.DataAccess.Abstractions.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static Kysect.Shreks.Application.Abstractions.Github.RemoveGithubSubjectCourseAssociation;

namespace Kysect.Shreks.Application.Handlers.Github;

public class RemoveGithubSubjectCourseAssociationHandler : IRequestHandler<Command, Response>
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
            .OfType<GithubSubjectCourseAssociation>()
            .Where(gsa => gsa.SubjectCourse.Id == request.SubjectCourseId)
            .FirstAsync(cancellationToken: cancellationToken);

        _context.SubjectCourseAssociations.Remove(subjectCourseAssociation);
        await _context.SaveChangesAsync(cancellationToken);

        SubjectCourse subjectCourse = await _context.SubjectCourses.GetByIdAsync(request.SubjectCourseId, cancellationToken);
        return new Response(SubjectCourseDtoFactory.CreateFrom(subjectCourse));
    }
}