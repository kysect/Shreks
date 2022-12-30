using Kysect.Shreks.Application.Contracts.Study.Commands;
using Kysect.Shreks.Application.Dto.SubjectCourses;
using Kysect.Shreks.Identity.Entities;
using Kysect.Shreks.WebApi.Abstractions.Models.SubjectCourseGroups;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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

    [HttpPost]
    public async Task<ActionResult<SubjectCourseGroupDto>> CreateSubjectCourseGroup(
        CreateSubjectCourseGroupRequest request)
    {
        (Guid subjectCourseId, Guid groupId) = request;

        var command = new CreateSubjectCourseGroup.Command(subjectCourseId, groupId);
        CreateSubjectCourseGroup.Response result = await _mediator.Send(command);

        return Ok(result);
    }

    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteSubjectCourseGroup(DeleteSubjectCourseGroupRequest request)
    {
        (Guid subjectCourseId, Guid groupId) = request;

        var command = new DeleteSubjectCourseGroup.Command(subjectCourseId, groupId);
        await _mediator.Send(command);

        return NoContent();
    }
}