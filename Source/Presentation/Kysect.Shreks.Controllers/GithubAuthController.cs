using Kysect.Shreks.Application.Contracts.Users.Commands;
using Kysect.Shreks.Identity.Entities;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Kysect.Shreks.Controllers;

[ApiController]
[Route("api/auth/github")]
[Authorize(Roles = ShreksIdentityRole.AdminRoleName)]
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

        string? githubUsername = userClaims
            .FindFirst("urn:github:url")?.Value
            .Split('/')
            .LastOrDefault();

        if (githubUsername is null)
            return Unauthorized();

        // TODO: use real UserId here
        var command = new UpdateUserGithubUsername.Command(UserId: Guid.NewGuid(), GithubUsername: githubUsername);
        var response = await _mediator.Send(command);

        return Ok(response);
    }
}