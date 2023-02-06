using Kysect.Shreks.Application.Contracts.Study.Assignments.Commands;
using Kysect.Shreks.Application.Contracts.Study.Assignments.Queries;
using Kysect.Shreks.Application.Contracts.Study.GroupAssignments.Commands;
using Kysect.Shreks.Application.Contracts.Study.GroupAssignments.Queries;
using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Identity.Entities;
using Kysect.Shreks.WebApi.Abstractions.Models;
using Kysect.Shreks.WebApi.Abstractions.Models.GroupAssignments;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kysect.Shreks.Controllers;

[Route("/api/[controller]")]
[ApiController]
[Authorize(Roles = ShreksIdentityRole.AdminRoleName)]
public class AssignmentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AssignmentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<ActionResult<AssignmentDto>> CreateAssignment(CreateAssignmentRequest request)
    {
        var command = new CreateAssignment.Command(
            request.SubjectCourseId,
            request.Title,
            request.ShortName,
            request.Order,
            request.MinPoints,
            request.MaxPoints);

        CreateAssignment.Response result = await _mediator.Send(command);
        return Ok(result.Assignment);
    }

    [HttpPatch("{id:guid}")]
    public async Task<ActionResult<AssignmentDto>> UpdateAssignmentPoints(Guid id, double minPoints, double maxPoints)
    {
        var command = new UpdateAssignmentPoints.Command(id, minPoints, maxPoints);

        UpdateAssignmentPoints.Response response = await _mediator.Send(command);

        return Ok(response.Assignment);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<AssignmentDto>> GetAssignmentById(Guid id)
    {
        var query = new GetAssignmentById.Query(id);

        GetAssignmentById.Response response = await _mediator.Send(query);

        return Ok(response.Assignment);
    }

    [HttpGet("{assignmentId:guid}/groups")]
    public async Task<ActionResult<IReadOnlyCollection<GroupAssignmentDto>>> Get(Guid assignmentId)
    {
        var query = new GetGroupAssignments.Query(assignmentId);
        GetGroupAssignments.Response response = await _mediator.Send(query);

        return Ok(response.GroupAssignments);
    }

    [HttpPut("{assignmentId:guid}/groups/{groupId:guid}")]
    public async Task<ActionResult<GroupAssignmentDto>> UpdateById(
        Guid assignmentId,
        Guid groupId,
        UpdateGroupAssignmentRequest request)
    {
        var deadline = DateOnly.FromDateTime(request.Deadline);
        var command = new UpdateGroupAssignmentDeadline.Command(groupId, assignmentId, deadline);

        UpdateGroupAssignmentDeadline.Response response = await _mediator.Send(command);

        return Ok(response.GroupAssignment);
    }
}