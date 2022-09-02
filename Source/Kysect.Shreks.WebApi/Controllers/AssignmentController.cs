using Kysect.Shreks.Application.Abstractions.Study.Commands;
using Kysect.Shreks.Application.Abstractions.Study.Queries;
using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Application.Handlers.Study.Assignments;
using Kysect.Shreks.WebApi.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Kysect.Shreks.WebApi.Controllers;

[Route("/api/[controller]")]
[ApiController]
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

        var result = await _mediator.Send(command);
        return Ok(result.Assignment);
    }

    [HttpPatch("{id:guid}")]
    public async Task<ActionResult<AssignmentDto>> UpdateAssignmentPoints(Guid id, double minPoints, double maxPoints)
    {
        var command = new UpdateAssignmentPoints.Command(id, minPoints, maxPoints);

        var response = await _mediator.Send(command);

        return Ok(response.Assignment);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<AssignmentDto>> GetAssignmentById(Guid id)
    {
        var query = new GetAssignmentById.Query(id);

        var response = await _mediator.Send(query);

        return Ok(response.Assignment);
    }

    [HttpGet("by-subject-course")]
    public async Task<ActionResult<AssignmentDto>> GetAssignmentBySubjectCourseId(Guid id)
    {
        var query = new GetAssignmentsBySubjectCourse.Query(id);

        var response = await _mediator.Send(query);

        return Ok(response.Assignments);
    }
}