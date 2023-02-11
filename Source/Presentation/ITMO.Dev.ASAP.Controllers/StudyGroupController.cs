using ITMO.Dev.ASAP.Application.Contracts.Students.Queries;
using ITMO.Dev.ASAP.Application.Contracts.Study.GroupAssignments.Queries;
using ITMO.Dev.ASAP.Application.Contracts.Study.StudyGroups.Commands;
using ITMO.Dev.ASAP.Application.Contracts.Study.StudyGroups.Queries;
using ITMO.Dev.ASAP.Application.Dto.Querying;
using ITMO.Dev.ASAP.Application.Dto.Study;
using ITMO.Dev.ASAP.Application.Dto.Users;
using ITMO.Dev.ASAP.Identity.Entities;
using ITMO.Dev.ASAP.WebApi.Abstractions.Models.StudyGroups;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ITMO.Dev.ASAP.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = AsapIdentityRole.AdminRoleName)]
public class StudyGroupController : ControllerBase
{
    private readonly IMediator _mediator;

    public StudyGroupController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<ActionResult<StudyGroupDto>> Create(CreateStudyGroupRequest request)
    {
        var command = new CreateStudyGroup.Command(request.Name);
        CreateStudyGroup.Response response = await _mediator.Send(command);

        return Ok(response.Group);
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<StudyGroupDto>>> Get()
    {
        var query = new GetStudyGroups.Query();
        GetStudyGroups.Response response = await _mediator.Send(query);

        return Ok(response.Groups);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<StudyGroupDto>> GetById(Guid id)
    {
        var query = new GetStudyGroupById.Query(id);
        GetStudyGroupById.Response response = await _mediator.Send(query);

        return Ok(response.Group);
    }

    [HttpGet("bulk")]
    public async Task<ActionResult<IReadOnlyCollection<StudyGroupDto>>> GetByIdsAsync(
        [FromQuery] IReadOnlyCollection<Guid> ids)
    {
        var query = new BulkGetStudyGroups.Query(ids);
        BulkGetStudyGroups.Response response = await _mediator.Send(query);

        return Ok(response.Groups);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<StudyGroupDto>> Update(Guid id, UpdateStudyGroupRequest request)
    {
        var command = new UpdateStudyGroup.Command(id, request.Name);
        UpdateStudyGroup.Response response = await _mediator.Send(command);

        return Ok(response.Group);
    }

    [HttpGet("{id:guid}/students")]
    public async Task<ActionResult<IReadOnlyCollection<StudentDto>>> GetStudentAsync(Guid id)
    {
        var query = new GetStudentsByGroupId.Query(id);
        GetStudentsByGroupId.Response response = await _mediator.Send(query);

        return Ok(response.Students);
    }

    [HttpGet("{groupId:guid}/assignments")]
    public async Task<ActionResult<GroupAssignmentDto>> GetAssignmentsAsync(Guid groupId)
    {
        var query = new GetGroupAssignmentsByStudyGroupId.Query(groupId);
        GetGroupAssignmentsByStudyGroupId.Response response = await _mediator.Send(query);

        return Ok(response.GroupAssignments);
    }

    [HttpGet("find")]
    public async Task<ActionResult<StudyGroupDto?>> FindByName(string name)
    {
        var query = new FindStudyGroupByName.Query(name);
        FindStudyGroupByName.Response response = await _mediator.Send(query);

        return Ok(response.Group);
    }

    [HttpPost("query")]
    public async Task<ActionResult<IReadOnlyCollection<StudyGroupDto>>> Query(
        QueryConfiguration<GroupQueryParameter> configuration)
    {
        var query = new FindGroupsByQuery.Query(configuration);
        FindGroupsByQuery.Response response = await _mediator.Send(query);

        return Ok(response.Groups);
    }
}