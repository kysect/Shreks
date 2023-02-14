using FluentAssertions;
using ITMO.Dev.ASAP.Core.Models;
using ITMO.Dev.ASAP.Core.Submissions;
using ITMO.Dev.ASAP.Core.Submissions.States;
using ITMO.Dev.ASAP.Core.ValueObject;
using ITMO.Dev.ASAP.Tests.Extensions;
using Xunit;

namespace ITMO.Dev.ASAP.Tests.Application;

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