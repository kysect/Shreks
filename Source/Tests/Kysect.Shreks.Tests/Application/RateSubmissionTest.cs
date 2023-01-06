using FluentAssertions;
using Kysect.Shreks.Core.Models;
using Kysect.Shreks.Core.Submissions;
using Kysect.Shreks.Core.Submissions.States;
using Kysect.Shreks.Core.ValueObject;
using Kysect.Shreks.Tests.Extensions;
using Xunit;

namespace Kysect.Shreks.Tests.Application;

public class RateSubmissionTest : TestBase
{
    [Fact]
    public async Task UpdateSubmission_Should_NoThrow()
    {
        Submission first = await Context.GetGithubSubmissionAsync(new ActiveSubmissionState());

        first.Rate(new Fraction(0.5), Points.None);
        first.State.Kind.Should().Be(SubmissionStateKind.Completed);

        first.UpdatePoints(new Fraction(0.5), Points.None);
        first.State.Kind.Should().Be(SubmissionStateKind.Completed);
    }
}