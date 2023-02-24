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

    public CancellationToken CancellationToken => HttpContext.RequestAborted;

    [HttpPost("force-update")]
    public async Task<ActionResult> ForceOrganizationUpdate()
    {
        var command = new UpdateSubjectCourseOrganizations.Command();
        await _mediator.Send(command, CancellationToken);
        return Ok();
    }

    [HttpPost("force-update")]
    public async Task<IActionResult> ForceOrganizationUpdateAsync(Guid subjectCourseId)
    {
        var command = new UpdateSubjectCourseOrganization.Command(subjectCourseId);
        await _mediator.Send(command, CancellationToken);

        return Ok();
    }

    [HttpPost("force-mentor-sync")]
    public async Task<ActionResult> ForceMentorsSync(string organizationName)
    {
        var command = new SyncGithubMentors.Command(organizationName);

        await _mediator.Send(command, CancellationToken);

        return Ok();
    }
}