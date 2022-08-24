using System.Security.Claims;
using Kysect.Shreks.Application.Abstractions.Students.Commands;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace Kysect.Shreks.WebApi.Controllers;

[ApiController]
[Route("api/auth/github")]
public class GithubAuthController : Controller
{
    private readonly IMediator _mediator;

    public GithubAuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("sign-in")]
    public IActionResult SignInGithubUser()
    {
        return Challenge(new AuthenticationProperties() { RedirectUri = "/" }, "GitHub");
    }

    [HttpPut("user")]
    public async Task<IActionResult> UpdateAuthorizedUserGithubUsername()
    {
        ClaimsPrincipal userClaims = HttpContext.User;

        String? githubUsername = userClaims
            .FindFirst("urn:github:url")?.Value
            .Split('/')
            .LastOrDefault();

        if (githubUsername is null)
            return BadRequest();

        // TODO: use real UserId here
        var command = new UpdateUserGithubUsername.Command(UserId: Guid.NewGuid(), GithubUsername: githubUsername);
        var response = await _mediator.Send(command);

        return Ok(response);
    }
}