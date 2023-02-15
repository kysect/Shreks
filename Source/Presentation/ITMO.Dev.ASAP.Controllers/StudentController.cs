using ITMO.Dev.ASAP.Application.Contracts.Github.Commands;
using ITMO.Dev.ASAP.Application.Contracts.Students.Commands;
using ITMO.Dev.ASAP.Application.Contracts.Students.Queries;
using ITMO.Dev.ASAP.Application.Contracts.Users.Commands;
using ITMO.Dev.ASAP.Application.Contracts.Users.Queries;
using ITMO.Dev.ASAP.Application.Dto.Querying;
using ITMO.Dev.ASAP.Application.Dto.Users;
using ITMO.Dev.ASAP.Identity.Entities;
using ITMO.Dev.ASAP.WebApi.Abstractions.Models.Students;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ITMO.Dev.ASAP.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = AsapIdentityRole.AdminRoleName)]
public class StudentController : ControllerBase
{
    private readonly IMediator _mediator;

    public StudentController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<ActionResult<StudentDto>> Create(CreateStudentRequest request)
    {
        (string? firstName, string? middleName, string? lastName, Guid groupId) = request;

        var command = new CreateStudent.Command(
            firstName ?? string.Empty,
            middleName ?? string.Empty,
            lastName ?? string.Empty,
            groupId);

        CreateStudent.Response response = await _mediator.Send(command);
        return Ok(response.Student);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<StudentDto>> GetById(Guid id)
    {
        GetStudentById.Response response = await _mediator.Send(new GetStudentById.Query(id));
        return Ok(response.Student);
    }

    [HttpPut("{id:guid}/dismiss")]
    public async Task<ActionResult> DismissFromGroup(Guid id)
    {
        var command = new DismissStudentFromGroup.Command(id);
        await _mediator.Send(command);

        return Ok();
    }

    [HttpPut("{id:guid}/group")]
    public async Task<ActionResult<StudentDto>> TransferStudentAsync(Guid id, TransferStudentRequest request)
    {
        var command = new TransferStudent.Command(id, request.NewGroupId);
        TransferStudent.Response response = await _mediator.Send(command);

        return Ok(response.Student);
    }

    [HttpPost("{id:guid}/association/github")]
    public async Task<ActionResult> AddGithubAssociation(Guid id, string githubUsername)
    {
        var command = new AddGithubUserAssociation.Command(id, githubUsername);
        await _mediator.Send(command);

        return Ok();
    }

    [HttpDelete("{id:guid}/association/github")]
    public async Task<ActionResult> RemoveGithubAssociation(Guid id)
    {
        var command = new RemoveGithubUserAssociation.Command(id);
        await _mediator.Send(command);

        return Ok();
    }

    [HttpPost("query")]
    public async Task<ActionResult<IReadOnlyCollection<StudentDto>>> Query(
        QueryConfiguration<StudentQueryParameter> configuration)
    {
        var query = new FindStudentsByQuery.Query(configuration);
        FindStudentsByQuery.Response response = await _mediator.Send(query);

        return Ok(response.Students);
    }
}