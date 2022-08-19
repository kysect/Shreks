using Kysect.Shreks.Application.Abstractions.Google.Models;
using MediatR;

namespace Kysect.Shreks.Application.Abstractions.Google.Queries;

public class GetStudentsQueueBySubjectCourse
{
    public record Query(Guid SubjectCourseId) : IRequest<Response>;

    public record Response(StudentsQueue Queue);
}