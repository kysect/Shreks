using ITMO.Dev.ASAP.Core.Study;
using ITMO.Dev.ASAP.Core.SubjectCourseAssociations;
using ITMO.Dev.ASAP.DataAccess.Abstractions;
using ITMO.Dev.ASAP.DataAccess.Abstractions.Extensions;
using ITMO.Dev.ASAP.Mapping.Mappings;
using MediatR;
using static ITMO.Dev.ASAP.Application.Contracts.Github.Commands.AddGithubSubjectCourseAssociation;

namespace ITMO.Dev.ASAP.Application.Handlers.Github.SubjectCourses;

internal class AddGithubSubjectCourseAssociationHandler : IRequestHandler<Command, Response>
{
    private readonly IDatabaseContext _context;

    public AddGithubSubjectCourseAssociationHandler(IDatabaseContext context)
    {
        _context = context;
    }

    public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
    {
        SubjectCourse subjectCourse = await _context.SubjectCourses
            .GetByIdAsync(request.SubjectCourseId, cancellationToken);

        var githubSubjectCourseAssociation = new GithubSubjectCourseAssociation(
            Guid.NewGuid(),
            subjectCourse,
            request.Organization,
            request.TemplateRepository);

        _context.SubjectCourseAssociations.Add(githubSubjectCourseAssociation);
        await _context.SaveChangesAsync(cancellationToken);

        return new Response(subjectCourse.ToDto());
    }
}