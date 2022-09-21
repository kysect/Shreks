using Kysect.Shreks.Application.Abstractions.Study.Commands;
using Kysect.Shreks.Application.Abstractions.Study.Queries;
using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Identity.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kysect.Shreks.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = ShreksIdentityRole.AdminRoleName)]
public class SubjectCourseGroupController : ControllerBase
{
    private readonly IMediator _mediator;

    public SubjectCourseGroupController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("by-subject-course-id")]
    public async Task<ActionResult<IReadOnlyCollection<SubjectCourseGroupDto>>> GetSubjectCourseGroupById(Guid id)
    {
        var query = new GetSubjectCourseGroupsBySubjectCourseId.Query(id);

        var result = await _mediator.Send(query);

        return Ok(result.Groups);
    }

    [HttpPost]
    public async Task<ActionResult<SubjectCourseGroupDto>> CreateSubjectCourseGroup(Guid subjectCourseId, Guid studentsGroupId)
    {
        var command = new CreateSubjectCourseGroup.Command(subjectCourseId, studentsGroupId);

        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteSubjectCourseGroup(Guid subjectCourseId, Guid studentsGroupId)
    {
        var command = new DeleteSubjectCourseGroup.Command(subjectCourseId, studentsGroupId);

        await _mediator.Send(command);
        return Ok();
    }
}