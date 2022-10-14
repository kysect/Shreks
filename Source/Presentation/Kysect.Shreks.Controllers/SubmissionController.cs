using Kysect.Shreks.Application.Abstractions.Study.Commands;
using Kysect.Shreks.Identity.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kysect.Shreks.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize(Roles = ShreksIdentityRole.AdminRoleName)]
public class SubmissionController : ControllerBase
{
    private readonly IMediator _mediator;

    public SubmissionController(IMediator mediator)
    {
        _mediator = mediator;
    }

    public CancellationToken CancellationToken => HttpContext.RequestAborted;

    [HttpPut("{id:guid}/compromise")]
    public async Task<IActionResult> CompromiseAsync(Guid id)
    {
        var command = new CompromiseSubmission.Command(id);

        await _mediator.Send(command, CancellationToken);

        return Ok();
    }
}