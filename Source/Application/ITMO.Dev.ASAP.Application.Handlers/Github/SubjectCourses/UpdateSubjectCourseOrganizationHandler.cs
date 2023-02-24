using ITMO.Dev.ASAP.Application.GithubWorkflow.Abstractions;
using MediatR;
using static ITMO.Dev.ASAP.Application.Contracts.Github.Commands.UpdateSubjectCourseOrganization;

namespace ITMO.Dev.ASAP.Application.Handlers.Github.SubjectCourses;

internal class UpdateSubjectCourseOrganizationHandler : IRequestHandler<Command>
{
    private readonly ISubjectCourseGithubOrganizationManager _subjectCourseGithubOrganizationManager;

    public UpdateSubjectCourseOrganizationHandler(
        ISubjectCourseGithubOrganizationManager subjectCourseGithubOrganizationManager)
    {
        _subjectCourseGithubOrganizationManager = subjectCourseGithubOrganizationManager;
    }

    public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
    {
        await _subjectCourseGithubOrganizationManager.UpdateSubjectCourseOrganizationAsync(
            request.SubjectCourseId,
            cancellationToken);

        return Unit.Value;
    }
}