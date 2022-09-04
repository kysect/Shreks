using Kysect.Shreks.Application.Abstractions.Students;
using Kysect.Shreks.Application.Abstractions.Users.Commands;
using Kysect.Shreks.Application.Abstractions.Users.Queries;
using Kysect.Shreks.Application.Dto.Users;
using Kysect.Shreks.Identity.Entities;
using Kysect.Shreks.WebApi.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kysect.Shreks.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = ShreksIdentityRole.AdminRoleName)]
public class UserController : ControllerBase
{
    private readonly IMediator _mediator;

    public UserController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("{userId:guid}/universityId/update")]
    public async Task<IActionResult> UpdateUniversityId(Guid userId, int universityId)
    {
        var caller = HttpContext.GetUser();
        var command = new UpdateUserUniversityId.Command(caller.Username, userId, universityId);
        await _mediator.Send(command);

        return Ok();
    }
    
    [HttpGet]
    [ProducesResponseType(200)]
    [ProducesResponseType(204)]
    public async Task<ActionResult<UserDto?>> FindUserByUniversityId(int universityId)
    {
        var command = new FindUserByUniversityId.Query(universityId);
        var user = await _mediator.Send(command);
        return Ok(user.User);
    }

    [HttpPost("{userId:guid}/change-name")]
    public async Task<ActionResult> UpdateName(Guid userId, string firstName, string middleName, string lastName)
    {
        await _mediator.Send(new UpdateUserName.Command(userId, firstName, middleName, lastName));
        return Ok();
    }
}