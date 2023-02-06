using Kysect.Shreks.Common.Exceptions;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.DataAccess.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Kysect.Shreks.Application.Extensions;

public static class SubjectCourseExtensions
{
    public static async Task<SubjectCourse> GetSubjectCourseByAssignmentId(
        this IShreksDatabaseContext context,
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