using System.Net;
using Kysect.Shreks.Identity.Entities;
using Kysect.Shreks.Integration.Github.Helpers;
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
    private readonly TestEnvironmentConfiguration _testEnvironmentConfiguration;

    public InternalController(IMediator mediator, TestEnvironmentConfiguration testEnvironmentConfiguration)
    {
        _mediator = mediator;
        _testEnvironmentConfiguration = testEnvironmentConfiguration;
    }

    [HttpPost("seed-test-data")]
    public async Task<IActionResult> SeedTestData([FromQuery] string environment)
    {
        var command = new Query(
            environment,
            _testEnvironmentConfiguration.Organization,
            _testEnvironmentConfiguration.TemplateRepository,
            _testEnvironmentConfiguration.Users);

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