using Kysect.Shreks.Application.Abstractions.Github;
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
public class SubjectCourseController : ControllerBase
{
    private readonly IMediator _mediator;

    public SubjectCourseController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<ActionResult<SubjectCourseDto>> Create(Guid subjectId, string name, SubmissionStateWorkflowTypeDto workflowType)
    {
        CreateSubjectCourse.Response response = await _mediator.Send(new CreateSubjectCourse.Command(subjectId, name, workflowType));
        return Ok(response.SubjectCourse);
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<SubjectCourseDto>>> Get()
    {
        GetSubjectCourses.Response response = await _mediator.Send(new GetSubjectCourses.Query());
        return Ok(response.Subjects);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<SubjectCourseDto>> GetById(Guid id)
    {
        GetSubjectCourseById.Response response = await _mediator.Send(new GetSubjectCourseById.Query(id));
        return Ok(response.SubjectCourse);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<SubjectCourseDto>> Update(Guid id, string name)
    {
        UpdateSubjectCourse.Response response = await _mediator.Send(new UpdateSubjectCourse.Command(id, name));
        return Ok(response.SubjectCourse);
    }

    [HttpPost("association/github")]
    public async Task<ActionResult<SubjectCourseDto>> AddGithubAssociation(Guid subjectCourseId, string organization, string templateRepository)
    {
        AddGithubSubjectCourseAssociation.Response response = await _mediator.Send(new AddGithubSubjectCourseAssociation.Command(subjectCourseId, organization, templateRepository));
        return Ok(response.SubjectCourse);
    }

    [HttpDelete("association/github")]
    public async Task<ActionResult<SubjectCourseDto>> RemoveGithubAssociation(Guid subjectCourseId)
    {
        RemoveGithubSubjectCourseAssociation.Response response = await _mediator.Send(new RemoveGithubSubjectCourseAssociation.Command(subjectCourseId));
        return Ok(response.SubjectCourse);
    }

    [HttpPost("deadline/fraction")]
    public async Task<ActionResult> AddDeadline(Guid subjectCourseId, TimeSpan spanBeforeActivation, double fraction)
    {
        var command = new AddFractionDeadlinePolicy.Command(subjectCourseId, spanBeforeActivation, fraction);
        AddFractionDeadlinePolicy.Response response = await _mediator.Send(command);
        return Ok();
    }
}