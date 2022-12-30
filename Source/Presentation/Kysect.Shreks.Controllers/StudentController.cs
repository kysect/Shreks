using Kysect.Shreks.Application.Contracts.Github.Commands;
using Kysect.Shreks.Application.Contracts.Students.Commands;
using Kysect.Shreks.Application.Contracts.Students.Queries;
using Kysect.Shreks.Application.Contracts.Users.Commands;
using Kysect.Shreks.Application.Contracts.Users.Queries;
using Kysect.Shreks.Application.Dto.Querying;
using Kysect.Shreks.Application.Dto.Users;
using Kysect.Shreks.Identity.Entities;
using Kysect.Shreks.WebApi.Abstractions.Models.Students;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kysect.Shreks.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = ShreksIdentityRole.AdminRoleName)]
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

        var command = new CreateStudent.Command
        (
            firstName ?? string.Empty,
            middleName ?? string.Empty,
            lastName ?? string.Empty, groupId
        );

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