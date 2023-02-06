using Kysect.Shreks.Application.Contracts.Github.Commands;
using Kysect.Shreks.Application.Contracts.Students.Queries;
using Kysect.Shreks.Application.Contracts.Study.Assignments.Queries;
using Kysect.Shreks.Application.Contracts.Study.SubjectCourseGroups.Queries;
using Kysect.Shreks.Application.Contracts.Study.SubjectCourses.Commands;
using Kysect.Shreks.Application.Contracts.Study.SubjectCourses.Queries;
using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Application.Dto.SubjectCourses;
using Kysect.Shreks.Application.Dto.Users;
using Kysect.Shreks.Identity.Entities;
using Kysect.Shreks.WebApi.Abstractions.Models.SubjectCourses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kysect.Shreks.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = ShreksIdentityRole.AdminRoleName)]
public class SubjectCourseController : ControllerBase
{
    private readonly IMediator _mediator;

    public SubjectCourseController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<ActionResult<SubjectCourseDto>> Create(CreateSubjectCourseRequest request)
    {
        var command = new CreateSubjectCourse.Command(
            request.SubjectId,
            request.Name,
            request.WorkflowType,
            request.Associations);

        CreateSubjectCourse.Response response = await _mediator.Send(command);

        return Ok(response.SubjectCourse);
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<SubjectCourseDto>>> Get()
    {
        var query = new GetSubjectCourses.Query();
        GetSubjectCourses.Response response = await _mediator.Send(query);

        return Ok(response.Subjects);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<SubjectCourseDto>> GetById(Guid id)
    {
        var query = new GetSubjectCourseById.Query(id);
        GetSubjectCourseById.Response response = await _mediator.Send(query);

        return Ok(response.SubjectCourse);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<SubjectCourseDto>> Update(Guid id, UpdateSubjectCourseRequest request)
    {
        var command = new UpdateSubjectCourse.Command(id, request.Name);
        UpdateSubjectCourse.Response response = await _mediator.Send(command);

        return Ok(response.SubjectCourse);
    }

    [HttpGet("{id:guid}/students")]
    public async Task<ActionResult<IReadOnlyCollection<StudentDto>>> GetStudentsAsync(Guid id)
    {
        var query = new GetStudentsBySubjectCourseId.Query(id);
        GetStudentsBySubjectCourseId.Response response = await _mediator.Send(query);

        return Ok(response.Students);
    }

    [HttpGet("{id:guid}/assignments")]
    public async Task<ActionResult<IReadOnlyCollection<AssignmentDto>>> GetAssignmentsBySubjectCourseId(Guid id)
    {
        var query = new GetAssignmentsBySubjectCourse.Query(id);
        GetAssignmentsBySubjectCourse.Response response = await _mediator.Send(query);

        return Ok(response.Assignments);
    }

    [HttpGet("{id:guid}/groups")]
    public async Task<ActionResult<IReadOnlyCollection<SubjectCourseGroupDto>>> GetSubjectCourseGroups(Guid id)
    {
        var query = new GetSubjectCourseGroupsBySubjectCourseId.Query(id);
        GetSubjectCourseGroupsBySubjectCourseId.Response result = await _mediator.Send(query);

        return Ok(result.Groups);
    }

    [HttpPost("{id:guid}/association/github")]
    public async Task<ActionResult<SubjectCourseDto>> AddGithubAssociation(
        Guid id,
        AddSubjectCourseGithubAssociationRequest request)
    {
        (string organizationName, string templateRepositoryName) = request;

        var command = new AddGithubSubjectCourseAssociation.Command(id, organizationName, templateRepositoryName);
        AddGithubSubjectCourseAssociation.Response response = await _mediator.Send(command);

        return Ok(response.SubjectCourse);
    }

    [HttpDelete("{id:guid}/association/github")]
    public async Task<ActionResult<SubjectCourseDto>> RemoveGithubAssociation(Guid id)
    {
        var command = new RemoveGithubSubjectCourseAssociation.Command(id);
        RemoveGithubSubjectCourseAssociation.Response response = await _mediator.Send(command);

        return Ok(response.SubjectCourse);
    }

    [HttpPost("{id:guid}/deadline/fraction")]
    public async Task<ActionResult> AddDeadline(Guid id, AddFractionPolicyRequest request)
    {
        (TimeSpan spanBeforeActivation, double fraction) = request;

        var command = new AddFractionDeadlinePolicy.Command(id, spanBeforeActivation, fraction);
        await _mediator.Send(command);

        return Ok();
    }
}