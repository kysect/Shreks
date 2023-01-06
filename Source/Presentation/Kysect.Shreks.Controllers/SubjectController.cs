using Kysect.Shreks.Application.Contracts.Study.Commands;
using Kysect.Shreks.Application.Contracts.Study.Queries;
using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Application.Dto.SubjectCourses;
using Kysect.Shreks.Identity.Entities;
using Kysect.Shreks.WebApi.Abstractions.Models.Subjects;
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
    public async Task<ActionResult<SubjectDto>> Create(CreateSubjectRequest request)
    {
        var command = new CreateSubject.Command(request.Name);
        CreateSubject.Response response = await _mediator.Send(command);

        return Ok(response.Subject);
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<SubjectDto>>> Get()
    {
        var query = new GetSubjects.Query();
        GetSubjects.Response response = await _mediator.Send(query);

        return Ok(response.Subjects);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<SubjectDto>> GetById(Guid id)
    {
        var query = new GetSubjectById.Query(id);
        GetSubjectById.Response response = await _mediator.Send(query);

        return Ok(response.Subject);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<SubjectDto>> Update(Guid id, UpdateSubjectRequest request)
    {
        var command = new UpdateSubject.Command(id, request.Name);
        UpdateSubject.Response response = await _mediator.Send(command);

        return Ok(response.Subject);
    }

    [HttpGet("{id:guid}/courses")]
    public async Task<ActionResult<IReadOnlyCollection<SubjectCourseDto>>> GetSubjectCourses(Guid id)
    {
        var request = new GetSubjectCoursesBySubjectCourseId.Query(id);
        GetSubjectCoursesBySubjectCourseId.Response
            response = await _mediator.Send(request, HttpContext.RequestAborted);

        return Ok(response.Courses);
    }
}