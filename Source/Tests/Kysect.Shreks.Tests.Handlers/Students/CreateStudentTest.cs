using FluentAssertions;
using Kysect.Shreks.Application.Contracts.Students.Commands;
using Kysect.Shreks.Application.Handlers.Students;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Kysect.Shreks.Tests.Handlers.Students;

public class CreateStudentTest : TestBase
{
    [Fact]
    public async Task Handle_Should_NotThrow()
    {
        Guid groupId = await Context.StudentGroups
            .Select(x => x.Id)
            .FirstAsync();

        var command = new CreateStudent.Command("A", "B", "C", groupId);
        var handler = new CreateStudentHandler(Context, Mapper);

        CreateStudent.Response response = await handler.Handle(command, default);

        response.Student.Should().NotBeNull();
    }
}