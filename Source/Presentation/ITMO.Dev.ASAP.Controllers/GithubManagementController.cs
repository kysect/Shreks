using ITMO.Dev.ASAP.Application.Contracts.Github.Commands;
using ITMO.Dev.ASAP.Identity.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ITMO.Dev.ASAP.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = AsapIdentityRole.AdminRoleName)]
public class GithubManagementController : ControllerBase
{
    private readonly IMediator _mediator;

    public GithubManagementController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("force-update")]
    public async Task<ActionResult> ForceOrganizationUpdate()
    {
        var updateOrganizationCommand = new UpdateSubjectCourseOrganizations.Command();
        await _mediator.Send(updateOrganizationCommand);
        return Ok();
    }

    [HttpPost("force-mentor-sync")]
    public async Task<ActionResult> ForceMentorsSync(string organizationName)
    {
        var command = new SyncGithubAdminWithMentors.Command(organizationName);

        await _mediator.Send(command);

        return Ok();
    }
}