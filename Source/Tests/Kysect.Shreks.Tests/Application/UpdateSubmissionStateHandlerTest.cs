using FluentAssertions;
using Kysect.Shreks.Application.Abstractions.Submissions.Commands;
using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Application.Handlers.Submissions;
using Kysect.Shreks.Core.Models;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Kysect.Shreks.Tests.Application;

public class UpdateSubmissionStateHandlerTest : ApplicationTestBase
{
    [Fact]
    public async Task Handle_Should_UpdateSubmissionState()
    {
        // Arrange
        const SubmissionStateDto stateDto = SubmissionStateDto.Active;
        const SubmissionState state = SubmissionState.Active;
        var submission = await Context.Submissions.FirstAsync();

        var command = new UpdateSubmissionState.Command(submission.Student.User.Id, submission.Id, stateDto);

        var handler = new UpdateSubmissionStateHandler(Context, Mapper);

        // Act
        var response = await handler.Handle(command, CancellationToken.None);
        
        // Assert
        response.Submission.State.Should().Be(stateDto);
        submission.State.Should().Be(state);
    }
}