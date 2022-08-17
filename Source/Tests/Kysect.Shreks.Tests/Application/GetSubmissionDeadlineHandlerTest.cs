using FluentAssertions;
using Kysect.Shreks.Application.Abstractions.Submissions.Queries;
using Kysect.Shreks.Application.Handlers.Submissions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Kysect.Shreks.Tests.Application;

public class GetSubmissionDeadlineHandlerTest : ApplicationTestBase
{
    [Fact]
    public async Task Handle_Should_ReturnDeadline()
    {
        // Arrange
        var submission = await Context.Submissions.FirstAsync();
        var query = new GetSubmissionDeadline.Query(submission.Id);
        var handler = new GetSubmissionDeadlineHandler(Context);
        
        // Act 
        var response = await handler.Handle(query, CancellationToken.None);
        
        // Assert
        response.Deadline.Should().NotBe(default);
    }
}