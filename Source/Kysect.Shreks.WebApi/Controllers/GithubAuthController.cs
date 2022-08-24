using System.Security.Claims;
using MediatR;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace Kysect.Shreks.WebApi.Controllers;

[ApiController]
[Route("api/auth/github")]
public class GithubAuthController : Controller
{
    [HttpGet("sign-in")]
    public IActionResult SignInGithubUser()
    {
        return Challenge(new AuthenticationProperties() { RedirectUri = "/" }, "GitHub");
    }
}