using Kysect.Shreks.Application.GithubWorkflow.Abstractions;
using Kysect.Shreks.DeveloperEnvironment;
using Kysect.Shreks.Identity.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Kysect.Shreks.Controllers;

[ApiController]
[Route("api/internal/")]
[Authorize(Roles = ShreksIdentityRole.AdminRoleName)]
public class InternalController : ControllerBase
{
    private readonly TestEnvironmentConfiguration _testEnvironmentConfiguration;
    private readonly DeveloperEnvironmentSeeder _developerEnvironmentSeeder;

    public InternalController(DeveloperEnvironmentSeeder developerEnvironmentSeeder, TestEnvironmentConfiguration testEnvironmentConfiguration)
    {
        _testEnvironmentConfiguration = testEnvironmentConfiguration;
        _developerEnvironmentSeeder = developerEnvironmentSeeder;
    }

    [HttpPost("seed-test-data")]
    public async Task<IActionResult> SeedTestData([FromQuery] string environment)
    {
        var command = new DeveloperEnvironmentSeedingRequest(
            environment,
            _testEnvironmentConfiguration.Organization,
            _testEnvironmentConfiguration.TemplateRepository,
            _testEnvironmentConfiguration.Users);

        try
        {
            await _developerEnvironmentSeeder.Handle(command, CancellationToken.None);
        }
        catch (UserNotAcknowledgedEnvironmentException e)
        {
            return StatusCode((int)HttpStatusCode.BadRequest, new ProblemDetails
            {
                Status = (int)HttpStatusCode.BadRequest,
                Title = e.Message,
                Detail = "You must put string 'Testing' into environment field if you have 100% ensured that it is not a production environment"
            });
        }

        return NoContent();
    }
}