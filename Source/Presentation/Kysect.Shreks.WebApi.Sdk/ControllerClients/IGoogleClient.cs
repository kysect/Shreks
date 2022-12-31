namespace Kysect.Shreks.WebApi.Sdk.ControllerClients;

public interface IGoogleClient
{
    Task ForceSubjectCourseTableSyncAsync(Guid subjectCourseId, CancellationToken cancellationToken = default);
}