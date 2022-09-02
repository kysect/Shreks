using Kysect.Shreks.Application.Abstractions.Github.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Kysect.Shreks.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GithubManagementController : ControllerBase
    {
        private readonly IMediator _mediator;

        public GithubManagementController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("force-update")]
        public async Task<ActionResult> ForceOrganizationUpdate()
        {
            var updateOrganizationCommand = new UpdateSubjectCourseOrganizations.Command();
            await _mediator.Send(updateOrganizationCommand);
            return Ok();
        }
    }
}
