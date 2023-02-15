using ITMO.Dev.ASAP.DeveloperEnvironment.Requests;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ITMO.Dev.ASAP.DeveloperEnvironment.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DeveloperController : Controller
{
    private readonly IMediator _mediator;

    public DeveloperController(IMediator mediator)
    {
        _mediator = mediator;
    }

    public CancellationToken CancellationToken => HttpContext.RequestAborted;

    public async Task<IActionResult> DeleteSubjectAsync(Guid subjectId)
    {
        var command = new DeleteSubject.Command(subjectId);
        await _mediator.Send(command, CancellationToken);

        return Ok();
    }
}