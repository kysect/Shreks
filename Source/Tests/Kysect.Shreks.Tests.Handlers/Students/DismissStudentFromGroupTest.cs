using Kysect.Shreks.Application.Contracts.Users.Commands;
using Kysect.Shreks.Application.Handlers.Students;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Kysect.Shreks.Tests.Handlers.Students;

public class DismissStudentFromGroupTest : TestBase
{
    [Fact]
    public async Task Handle_Should_NotThrow()
    {
        Guid studentId = await Context.Students
            .Where(x => x.Group != null)
            .Select(x => x.UserId)
            .FirstAsync();

        var command = new DismissStudentFromGroup.Command(studentId);
        var handler = new DismissStudentFromGroupHandler(Context);

        await handler.Handle(command, default);
    }
}