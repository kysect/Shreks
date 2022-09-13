using FluentAssertions;
using Kysect.Shreks.Application.Commands.Processors;
using Kysect.Shreks.Core.Models;
using Kysect.Shreks.Core.Submissions;
using Kysect.Shreks.Integration.Google;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Kysect.Shreks.Tests.Application;

public class UpdateSubmissionStateHandlerTest : ApplicationTestBase
{
    [Fact]
    public async Task Handle_Should_UpdateSubmissionState()
    {
        // Arrange
        var submissionService = new SubmissionService(Context, new TableUpdateQueue());
        const SubmissionState stateDto = SubmissionState.Active;
        const SubmissionState state = SubmissionState.Active;
        var submission = await Context.Submissions
            .Where(s => s.State != SubmissionState.Completed)
            .FirstAsync();

        // Act
        Submission response = await submissionService.UpdateSubmissionState(submission.Id, submission.Student.UserId, state, CancellationToken.None);

        // Assert
        response.State.Should().Be(stateDto);
        submission.State.Should().Be(state);
    }
}