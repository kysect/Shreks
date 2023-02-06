using Kysect.Shreks.Common.Exceptions;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.DataAccess.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Kysect.Shreks.Application.Extensions;

public static class StudentGroupExtensions
{
    public static async Task<StudentGroup> GetStudentGroupByStudentId(
        this IShreksDatabaseContext context,
        Guid studentId,
        CancellationToken cancellationToken)
    {
        StudentGroup? group = await context.Students
            .Where(x => x.UserId.Equals(studentId))
            .Select(x => x.Group)
            .SingleOrDefaultAsync(cancellationToken);

        if (group is null)
        {
            throw new EntityNotFoundException($"Group for student {studentId} not found");
        }

        return group;
    }
}