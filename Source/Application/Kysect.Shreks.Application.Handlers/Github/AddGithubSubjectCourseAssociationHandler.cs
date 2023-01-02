using Kysect.Shreks.Application.Factories;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Core.SubjectCourseAssociations;
using Kysect.Shreks.DataAccess.Abstractions;
using Kysect.Shreks.DataAccess.Abstractions.Extensions;
using MediatR;
using static Kysect.Shreks.Application.Contracts.Github.Commands.AddGithubSubjectCourseAssociation;

namespace Kysect.Shreks.Application.Handlers.Github;

internal class AddGithubSubjectCourseAssociationHandler : IRequestHandler<Command, Response>
{
    private readonly IShreksDatabaseContext _context;

    public AddGithubSubjectCourseAssociationHandler(IShreksDatabaseContext context)
    {
        _context = context;
    }

    public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
    {
        SubjectCourse subjectCourse =
            await _context.SubjectCourses.GetByIdAsync(request.SubjectCourseId, cancellationToken);
        var githubSubjectCourseAssociation =
            new GithubSubjectCourseAssociation(subjectCourse, request.Organization, request.TemplateRepository);

        _context.SubjectCourseAssociations.Add(githubSubjectCourseAssociation);
        await _context.SaveChangesAsync(cancellationToken);

        subjectCourse = await _context.SubjectCourses.GetByIdAsync(request.SubjectCourseId, cancellationToken);
        return new Response(SubjectCourseDtoFactory.CreateFrom(subjectCourse));
    }
}