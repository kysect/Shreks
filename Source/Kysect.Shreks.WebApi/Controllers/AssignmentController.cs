using Kysect.Shreks.Application.Abstractions.Study.Commands;
using Kysect.Shreks.Application.Dto.Study;
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
}