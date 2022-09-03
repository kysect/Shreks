using Kysect.Shreks.Integration.Google;
using Microsoft.AspNetCore.Mvc;

namespace Kysect.Shreks.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GoogleController : ControllerBase
    {
        private readonly TableUpdateQueue _tableUpdate;

        public GoogleController(TableUpdateQueue tableUpdate)
        {
            _tableUpdate = tableUpdate;
        }

        [HttpPost("force-sync")]
        public Task<ActionResult> ForceSubjectCourseTableSyncAsync(Guid subjectCourseId)
        {
            _tableUpdate.EnqueueCoursePointsUpdate(subjectCourseId);
            return Task.FromResult<ActionResult>(Ok());
        }
    }
}
