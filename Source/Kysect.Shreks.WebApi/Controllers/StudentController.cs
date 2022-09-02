using Kysect.Shreks.Application.Abstractions.Github.Commands;
using Kysect.Shreks.Application.Abstractions.Students;
using Kysect.Shreks.Application.Dto.Users;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Kysect.Shreks.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly IMediator _mediator;

        public StudentController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<ActionResult<StudentDto>> Create(string firstName, string middleName, string lastName, Guid groupId, int universityId)
        {
            CreateStudent.Response response = await _mediator.Send(new CreateStudent.Command(firstName, middleName, lastName, groupId, universityId));
            return Ok(response.Student);
        }

        [HttpGet("by-group")]
        public async Task<ActionResult<IReadOnlyCollection<StudentDto>>> Get(Guid groupId)
        {
            GetStudentsByGroupId.Response response = await _mediator.Send(new GetStudentsByGroupId.Query(groupId));
            return Ok(response.Students);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<StudentDto>> GetById(Guid id)
        {
            GetStudentById.Response response = await _mediator.Send(new GetStudentById.Query(id));
            return Ok(response.Student);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            DeleteStudent.Response response = await _mediator.Send(new DeleteStudent.Command(id));
            return Ok();
        }

        [HttpPost("association/github")]
        public async Task<ActionResult> AddGithubAssociation(Guid userId, string githubUsername)
        {
            AddGithubUserAssociation.Response response = await _mediator.Send(new AddGithubUserAssociation.Command(userId, githubUsername));
            return Ok();
        }

        [HttpDelete("association/github")]
        public async Task<ActionResult> RemoveGithubAssociation(Guid userId)
        {
            RemoveGithubUserAssociation.Response response = await _mediator.Send(new RemoveGithubUserAssociation.Command(userId));
            return Ok();
        }
    }
}
