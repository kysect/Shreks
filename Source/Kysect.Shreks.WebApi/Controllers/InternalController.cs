using System.Net;
using Kysect.Shreks.Identity.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Kysect.Shreks.Application.Abstractions.Internal.SeedTestData;

namespace Kysect.Shreks.WebApi.Controllers;

[ApiController]
[Route("api/internal/")]
[Authorize(Roles = ShreksIdentityRole.AdminRoleName)]
public class InternalController : ControllerBase
{
    private readonly IMediator _mediator;

    public InternalController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("seed-test-data")]
    public async Task<IActionResult> SeedTestData([FromQuery] string environment)
    {
        var command = new Query(environment);
        try
        {
            await _mediator.Send(command);
        }
        catch (UserNotAcknowledgedEnvironmentException e)
        {
            return StatusCode((int) HttpStatusCode.BadRequest, new ProblemDetails
            {
                Status = (int) HttpStatusCode.BadRequest,
                Title = e.Message,
                Detail = "You must put string 'Testing' into environment field if you have 100% ensured that it is not a production environment"
            });
        }

        return NoContent();
    }
}