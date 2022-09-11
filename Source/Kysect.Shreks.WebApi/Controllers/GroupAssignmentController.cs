using Kysect.Shreks.Application.Abstractions.Study.Commands;
using Kysect.Shreks.Application.Abstractions.Study.Queries;
using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Identity.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kysect.Shreks.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = ShreksIdentityRole.AdminRoleName)]
    public class GroupAssignmentController : ControllerBase
    {
        private readonly IMediator _mediator;

        public GroupAssignmentController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<ActionResult<GroupAssignmentDto>> Create(Guid groupId, Guid assignmentId, DateTime deadline)
        {
            CreateGroupAssignment.Response response = await _mediator.Send(new CreateGroupAssignment.Command(groupId, assignmentId, DateOnly.FromDateTime(deadline)));
            return Ok(response.GroupAssignment);
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyCollection<GroupAssignmentDto>>> Get()
        {
            GetGroupAssignments.Response response = await _mediator.Send(new GetGroupAssignments.Query());
            return Ok(response.GroupAssignments);
        }

        [HttpGet("groups/{groupId}")]
        public async Task<ActionResult<GroupAssignmentDto>> GetByGroupId(Guid groupId)
        {
            GetGroupAssignmentsByStudyGroupId.Response response = await _mediator.Send(new GetGroupAssignmentsByStudyGroupId.Query(groupId));
            return Ok(response.GroupAssignments);
        }

        [HttpPut("groups/{groupId}")]
        public async Task<ActionResult<GroupAssignmentDto>> UpdateById(Guid groupId, Guid assignmentId, DateOnly newDeadline)
        {
            UpdateGroupAssignmentDeadline.Response response = await _mediator.Send(new UpdateGroupAssignmentDeadline.Command(groupId, assignmentId, newDeadline));
            return Ok(response.GroupAssignment);
        }
    }
}