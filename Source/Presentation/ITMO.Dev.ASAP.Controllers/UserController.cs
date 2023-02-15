using ITMO.Dev.ASAP.Application.Contracts.Students.Commands;
using ITMO.Dev.ASAP.Application.Contracts.Users.Commands;
using ITMO.Dev.ASAP.Application.Contracts.Users.Queries;
using ITMO.Dev.ASAP.Application.Dto.Identity;
using ITMO.Dev.ASAP.Application.Dto.Users;
using ITMO.Dev.ASAP.Controllers.Extensions;
using ITMO.Dev.ASAP.Identity.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ITMO.Dev.ASAP.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = AsapIdentityRole.AdminRoleName)]
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
        IdentityUserDto caller = HttpContext.GetUser();
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
        FindUserByUniversityId.Response user = await _mediator.Send(command);
        return Ok(user.User);
    }

    [HttpPost("{userId:guid}/change-name")]
    public async Task<ActionResult> UpdateName(Guid userId, string firstName, string middleName, string lastName)
    {
        await _mediator.Send(new UpdateUserName.Command(userId, firstName, middleName, lastName));
        return Ok();
    }
}