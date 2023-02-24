using ITMO.Dev.ASAP.Application.GithubWorkflow.Abstractions;
using MediatR;
using static ITMO.Dev.ASAP.Application.Contracts.Github.Commands.UpdateSubjectCourseOrganizations;

namespace ITMO.Dev.ASAP.Application.Handlers.Github.SubjectCourses;

internal class UpdateSubjectCourseOrganizationsHandler : IRequestHandler<Command, Response>
{
    private readonly ISubjectCourseGithubOrganizationManager _subjectCourseGithubOrganizationManager;

    public UpdateSubjectCourseOrganizationsHandler(
        ISubjectCourseGithubOrganizationManager subjectCourseGithubOrganizationManager)
    {
        _subjectCourseGithubOrganizationManager = subjectCourseGithubOrganizationManager;
    }

    public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
    {
        await _subjectCourseGithubOrganizationManager.UpdateOrganizationsAsync(cancellationToken);

        return new Response();
    }
}