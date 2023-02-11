using ITMO.Dev.ASAP.Common.Exceptions;
using ITMO.Dev.ASAP.Core.Study;
using ITMO.Dev.ASAP.DataAccess.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace ITMO.Dev.ASAP.Application.Extensions;

public static class SubjectCourseExtensions
{
    public static async Task<SubjectCourse> GetSubjectCourseByAssignmentId(
        this IDatabaseContext context,
        Guid assignmentId,
        CancellationToken cancellationToken)
    {
        SubjectCourse? subjectCourse = await context.Assignments
            .Where(x => x.Id.Equals(assignmentId))
            .Select(x => x.SubjectCourse)
            .SingleOrDefaultAsync(cancellationToken);

        if (subjectCourse is null)
        {
            string message = $"Subject course for assignment {assignmentId} not found";
            throw new EntityNotFoundException(message);
        }

        return subjectCourse;
    }
}