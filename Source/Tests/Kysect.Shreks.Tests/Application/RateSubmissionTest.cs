using FluentAssertions;
using Kysect.Shreks.Core.Models;
using Kysect.Shreks.Core.Submissions;
using Kysect.Shreks.Core.ValueObject;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Kysect.Shreks.Tests.Application;

public class RateSubmissionTest : ApplicationTestBase
{
    [Fact]
    public async Task UpdateSubmission_Should_NoThrow()
    {
        Submission first = await Context
            .Submissions
            .Where(s => s.Rating == null)
            .FirstAsync();

        first.Rate(new Fraction(0.5), Points.None);
        first.State.Kind.Should().Be(SubmissionStateKind.Completed);

        first.UpdatePoints(new Fraction(0.5), Points.None);
        first.State.Kind.Should().Be(SubmissionStateKind.Completed);
    }
}