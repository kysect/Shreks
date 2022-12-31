using Kysect.Shreks.Application.Contracts.Study.Commands;
using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Identity.Entities;
using Kysect.Shreks.WebApi.Abstractions.Models.GroupAssignments;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kysect.Shreks.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = ShreksIdentityRole.AdminRoleName)]
public class GroupAssignmentController : ControllerBase
{
    private readonly IMediator _mediator;

    public GroupAssignmentController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<ActionResult<GroupAssignmentDto>> Create(CreateGroupAssignmentRequest request)
    {
        (Guid groupId, Guid assignmentId, DateTime deadline) = request;

        var command = new CreateGroupAssignment.Command(groupId, assignmentId, DateOnly.FromDateTime(deadline));
        CreateGroupAssignment.Response response = await _mediator.Send(command);

        return Ok(response.GroupAssignment);
    }
}