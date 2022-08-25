using FluentAssertions;
using Kysect.Shreks.Application.Abstractions.Submissions.Commands;
using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Application.Handlers.Submissions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Kysect.Shreks.Tests.Application;

public class UpdateSubmissionStateHandlerTest : ApplicationTestBase
{
    [Fact]
    public async Task Handle_ShouldUpdateState()
    {
        // Arrange
        var submission = await Context.Submissions.FirstAsync();
        var state = SubmissionStateDto.Active;

        var command = new UpdateSubmissionState.Command(submission.Student.User.Id, submission.Id, state);

        var handler = new UpdateSubmissionStateHandler(Context, Mapper);

        // Act
        var response = await handler.Handle(command, CancellationToken.None);
        
        // Assert
        response.Submission.State.Should().Be(state);
    }
}