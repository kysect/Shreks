using Kysect.Shreks.Application.Abstractions.Study.Commands;
using Kysect.Shreks.Application.Abstractions.Study.Queries;
using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Identity.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kysect.Shreks.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = ShreksIdentityRole.AdminRoleName)]
public class StudyGroupController : ControllerBase
{
    private readonly IMediator _mediator;

    public StudyGroupController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<ActionResult<StudyGroupDto>> Create(string name)
    {
        CreateStudyGroup.Response response = await _mediator.Send(new CreateStudyGroup.Command(name));
        return Ok(response.Group);
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<StudyGroupDto>>> Get()
    {
        GetStudyGroups.Response response = await _mediator.Send(new GetStudyGroups.Query());
        return Ok(response.Groups);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<StudyGroupDto>> GetById(Guid id)
    {
        GetStudyGroupById.Response response = await _mediator.Send(new GetStudyGroupById.Query(id));
        return Ok(response.Group);
    }

    [HttpGet("search")]
    public async Task<ActionResult<StudyGroupDto>> FindByName(string name)
    {
        FindStudyGroupByName.Response response = await _mediator.Send(new FindStudyGroupByName.Query(name));
        return Ok(response.Group);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<StudyGroupDto>> Update(Guid id, string name)
    {
        UpdateStudyGroup.Response response = await _mediator.Send(new UpdateStudyGroup.Command(id, name));
        return Ok(response.Group);
    }
}