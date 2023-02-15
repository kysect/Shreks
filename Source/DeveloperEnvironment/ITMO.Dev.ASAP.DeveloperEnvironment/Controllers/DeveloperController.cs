using ITMO.Dev.ASAP.DeveloperEnvironment.Requests;
using ITMO.Dev.ASAP.Identity.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ITMO.Dev.ASAP.DeveloperEnvironment.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = AsapIdentityRole.AdminRoleName)]
public class DeveloperController : Controller
{
    private readonly IMediator _mediator;

    public DeveloperController(IMediator mediator)
    {
        _mediator = mediator;
    }

    public CancellationToken CancellationToken => HttpContext.RequestAborted;

    [HttpDelete("subjects/{subjectId:guid}")]
    public async Task<IActionResult> DeleteSubjectAsync(Guid subjectId)
    {
        var command = new DeleteSubject.Command(subjectId);
        await _mediator.Send(command, CancellationToken);

        return Ok();
    }
}