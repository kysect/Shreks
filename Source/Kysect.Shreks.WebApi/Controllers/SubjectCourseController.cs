using Kysect.Shreks.Application.Abstractions.Github.Commands;
using Kysect.Shreks.Application.Abstractions.Study.Commands;
using Kysect.Shreks.Application.Abstractions.Study.Queries;
using Kysect.Shreks.Application.Dto.Study;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Kysect.Shreks.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubjectCourseController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SubjectCourseController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<ActionResult<SubjectCourseDto>> Create(Guid subjectId, string name)
        {
            CreateSubjectCourse.Response response = await _mediator.Send(new CreateSubjectCourse.Command(subjectId, name));
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
    }
}
