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
public class SubjectController : ControllerBase
{
    private readonly IMediator _mediator;

    public SubjectController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<ActionResult<SubjectDto>> Create(string name)
    {
        CreateSubject.Response response = await _mediator.Send(new CreateSubject.Command(name));
        return Ok(response.Subject);
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<SubjectDto>>> Get()
    {
        GetSubjects.Response response = await _mediator.Send(new GetSubjects.Query());
        return Ok(response.Subjects);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<SubjectDto>> GetById(Guid id)
    {
        GetSubjectById.Response response = await _mediator.Send(new GetSubjectById.Query(id));
        return Ok(response.Subject);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<SubjectDto>> Update(Guid id, string name)
    {
        UpdateSubject.Response response = await _mediator.Send(new UpdateSubject.Command(id, name));
        return Ok(response.Subject);
    }

    [HttpGet("{id:guid}/courses")]
    public async Task<ActionResult<IReadOnlyCollection<SubjectCourseDto>>> GetSubjectCourses(Guid id)
    {
        var request = new GetSubjectCoursesBySubjectCourseId.Query(id);
        GetSubjectCoursesBySubjectCourseId.Response response = await _mediator.Send(request, HttpContext.RequestAborted);

        return Ok(response.Courses);
    }
}