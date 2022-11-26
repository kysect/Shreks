namespace Kysect.Shreks.Application.Abstractions.Google;

public interface ISubjectCourseTableService
{
    Task<string> GetSubjectCourseTableId(Guid subjectCourseId, CancellationToken cancellationToken);
}