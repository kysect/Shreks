namespace Kysect.Shreks.Application.Abstractions.Google;

public interface IGoogleTableAccessorWorker
{
    void AddCourseToQueueUpdate(Guid subjectCourseId);
    void AddCourseToPointsUpdate(Guid subjectCourseId);
}