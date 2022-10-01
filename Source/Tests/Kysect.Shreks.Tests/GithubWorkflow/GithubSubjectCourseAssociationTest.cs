using FluentAssertions;
using Kysect.Shreks.Application.Commands.Parsers;
using Kysect.Shreks.Application.DatabaseContextExtensions;
using Kysect.Shreks.Application.GithubWorkflow;
using Kysect.Shreks.Application.GithubWorkflow.Abstractions;
using Kysect.Shreks.Application.GithubWorkflow.Abstractions.Models;
using Kysect.Shreks.Application.GithubWorkflow.Submissions;
using Kysect.Shreks.Application.TableManagement;
using Kysect.Shreks.Core.Models;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Core.SubjectCourseAssociations;
using Kysect.Shreks.Core.Submissions;
using Kysect.Shreks.Core.Tools;
using Kysect.Shreks.Core.UserAssociations;
using Kysect.Shreks.Core.Users;
using Kysect.Shreks.Tests.GithubWorkflow.Tools;
using Kysect.Shreks.Tests.Tools;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;

namespace Kysect.Shreks.Tests.GithubWorkflow;

public class GithubSubjectCourseAssociationTest : GithubWorkflowTestBase
{
    private readonly ILogger _logger;

    public GithubSubjectCourseAssociationTest(ITestOutputHelper output)
    {
        _logger = LogInitialization.InitTestLogger(output);
    }

    [Fact]
    public async Task GetSubjectCourseGithubUser_StudentAssociationExists_AssociationShouldReturn()
    {
        (GithubSubjectCourseAssociation subjectCourseAssociation, Student student) = await TestContextGenerator.Create();
        GithubUserAssociation githubUserAssociation = student.User.Associations.OfType<GithubUserAssociation>().First();

        IReadOnlyCollection<GithubUserAssociation> githubUserAssociations = await Context
            .SubjectCourses
            .GetAllGithubUsers(subjectCourseAssociation.SubjectCourse.Id);

        IEnumerable<string> organizationUsers = githubUserAssociations.Select(a => a.GithubUsername);

        organizationUsers.Should().Contain(githubUserAssociation.GithubUsername);
    }

    [Fact]
    public async Task PullRequestCreated_SubmissionShouldBeCreated()
    {
        var pullRequestCommitEventNotifier = new TestEventNotifier();
        var githubSubmissionService = new GithubSubmissionService(Context);
        IShreksWebhookEventProcessor githubSubmissionStateMachine = CreateEventProcessor();

        (GithubSubjectCourseAssociation subjectCourseAssociation, Student student) = await TestContextGenerator.Create();
        GithubUserAssociation githubUserAssociation = student.User.Associations.OfType<GithubUserAssociation>().First();
        Assignment assignment = subjectCourseAssociation.SubjectCourse.Assignments.First();

        var githubPullRequestDescriptor = new GithubPullRequestDescriptor(
            githubUserAssociation.GithubUsername,
            string.Empty,
            subjectCourseAssociation.GithubOrganizationName,
            githubUserAssociation.GithubUsername,
            assignment.ShortName,
            1);


        await githubSubmissionStateMachine.ProcessPullRequestUpdate(githubPullRequestDescriptor, _logger, pullRequestCommitEventNotifier, CancellationToken.None);
        Submission lastSubmissionByPr = await githubSubmissionService.GetLastSubmissionByPr(githubPullRequestDescriptor);

        Assert.Equal(SubmissionState.Active, lastSubmissionByPr.State);
    }

    [Fact]
    public async Task PullRequestUpdate_AfterSubmissionWasCreated_SubmissionShouldUpdateTime()
    {
        var pullRequestCommitEventNotifier = new TestEventNotifier();
        var githubSubmissionService = new GithubSubmissionService(Context);
        IShreksWebhookEventProcessor githubSubmissionStateMachine = CreateEventProcessor();

        (GithubSubjectCourseAssociation subjectCourseAssociation, Student student) = await TestContextGenerator.Create();
        GithubUserAssociation githubUserAssociation = student.User.Associations.OfType<GithubUserAssociation>().First();
        Assignment assignment = subjectCourseAssociation.SubjectCourse.Assignments.First();

        var githubPullRequestDescriptor = new GithubPullRequestDescriptor(
            githubUserAssociation.GithubUsername,
            string.Empty,
            subjectCourseAssociation.GithubOrganizationName,
            githubUserAssociation.GithubUsername,
            assignment.ShortName,
            1);

        await githubSubmissionStateMachine.ProcessPullRequestUpdate(githubPullRequestDescriptor, _logger, pullRequestCommitEventNotifier, CancellationToken.None);
        Submission createdSubmission = await githubSubmissionService.GetLastSubmissionByPr(githubPullRequestDescriptor);
        // Do not inline, it lead to test failing
        SpbDateTime submissionCreatedTime = createdSubmission.SubmissionDate;

        await githubSubmissionStateMachine.ProcessPullRequestUpdate(githubPullRequestDescriptor, _logger, pullRequestCommitEventNotifier, CancellationToken.None);
        Submission updatedSubmission = await githubSubmissionService.GetLastSubmissionByPr(githubPullRequestDescriptor);

        Assert.Equal(updatedSubmission.Id, createdSubmission.Id);
        Assert.NotEqual(updatedSubmission.SubmissionDate, submissionCreatedTime);
    }

    public ShreksWebhookEventProcessor CreateEventProcessor()
    {
        var githubSubmissionFactory = new GithubSubmissionFactory(Context);

        return new ShreksWebhookEventProcessor(
            new ShreksCommandParser(),
            Context,
            new TableUpdateQueue(),
            githubSubmissionFactory);
    }
}