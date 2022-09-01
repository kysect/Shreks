using Kysect.Shreks.Application.Abstractions.Identity.Commands;
using Kysect.Shreks.Application.Abstractions.Identity.Queries;
using Kysect.Shreks.Identity.Entities;
using Kysect.Shreks.WebApi.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kysect.Shreks.WebApi.Controllers;

[ApiController]
[Route("api/identity")]
public class IdentityController : ControllerBase
{
    private readonly IMediator _mediator;

    public IdentityController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync([FromBody] LoginRequest request)
    {
        var query = new Login.Query(request.Username, request.Password);
        var response = await _mediator.Send(query, HttpContext.RequestAborted);

        var loginResponse = new LoginResponse(response.Token, response.Expires, response.Roles);
        return Ok(loginResponse);
    }

    [HttpPost("users/{username}/promote")]
    [Authorize(Roles = ShreksIdentityRole.AdminRoleName)]
    public async Task<IActionResult> PromoteAsync(string username)
    {
        var command = new PromoteToAdmin.Command(username);
        await _mediator.Send(command);

        return Ok();
    }

    [HttpPost("register")]
    [Authorize(Roles = ShreksIdentityRole.AdminRoleName)]
    public async Task<IActionResult> RegisterAsync([FromBody] RegisterUserRequest request)
    {
        var command = new Register.Command(request.Username, request.Password);
        await _mediator.Send(command);

        return Ok();
    }
}