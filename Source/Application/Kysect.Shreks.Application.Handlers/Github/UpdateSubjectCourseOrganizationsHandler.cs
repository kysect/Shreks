using Kysect.Shreks.Application.GithubWorkflow.Abstractions;
using MediatR;
using static Kysect.Shreks.Application.Contracts.Github.Commands.UpdateSubjectCourseOrganizations;

namespace Kysect.Shreks.Application.Handlers.Github;

public class UpdateSubjectCourseOrganizationsHandler : IRequestHandler<Command, Response>
{
    private readonly ISubjectCourseGithubOrganizationManager _subjectCourseGithubOrganizationManager;

    public UpdateSubjectCourseOrganizationsHandler(ISubjectCourseGithubOrganizationManager subjectCourseGithubOrganizationManager)
    {
        _subjectCourseGithubOrganizationManager = subjectCourseGithubOrganizationManager;
    }

    public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
    {
        await _subjectCourseGithubOrganizationManager.UpdateOrganizations(cancellationToken);

        return new Response();
    }
}