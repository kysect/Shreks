using ITMO.Dev.ASAP.Common.Exceptions;
using ITMO.Dev.ASAP.Core.Study;
using ITMO.Dev.ASAP.DataAccess.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace ITMO.Dev.ASAP.Application.Extensions;

public static class StudentGroupExtensions
{
    public static async Task<StudentGroup> GetStudentGroupByStudentId(
        this IDatabaseContext context,
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