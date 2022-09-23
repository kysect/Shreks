using FluentAssertions;
using Kysect.Shreks.Application.Commands.Processors;
using Kysect.Shreks.Application.DatabaseContextExtensions;
using Kysect.Shreks.Application.GithubWorkflow;
using Kysect.Shreks.Application.GithubWorkflow.Abstractions.Models;
using Kysect.Shreks.Application.GithubWorkflow.Submissions;
using Kysect.Shreks.Application.GithubWorkflow.SubmissionStateMachines;
using Kysect.Shreks.Application.TableManagement;
using Kysect.Shreks.Core.Models;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Core.SubjectCourseAssociations;
using Kysect.Shreks.Core.Submissions;
using Kysect.Shreks.Core.UserAssociations;
using Kysect.Shreks.Core.Users;
using Kysect.Shreks.Tests.GithubWorkflow.Tools;
using Kysect.Shreks.Tests.Tools;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Kysect.Shreks.Tests.GithubWorkflow;

public class GithubSubjectCourseAssociationTest : GithubWorkflowTestBase
{
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

    [Fact(Skip = "TODO: implement 'open PR' action")]
    public async Task PullRequestCreated_SubmissionShouldBeCreated()
    {
        var githubSubmissionService = new GithubSubmissionService(Context);

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

        IGithubSubmissionStateMachine githubSubmissionStateMachine = CreateSubmissionStateMachine(githubPullRequestDescriptor);

        await githubSubmissionStateMachine.ProcessPullRequestReopen(false, githubPullRequestDescriptor);
        Submission lastSubmissionByPr = await githubSubmissionService.GetLastSubmissionByPr(githubPullRequestDescriptor);

        Assert.Equal(SubmissionState.Active, lastSubmissionByPr.State);
    }

    public IGithubSubmissionStateMachine CreateSubmissionStateMachine(GithubPullRequestDescriptor githubPullRequestDescriptor)
    {
        ILogger logger = LogInitialization.GetLogger();
        SubmissionService shreksCommandProcessor = new SubmissionService(Context, new TableUpdateQueue());

        var githubSubmissionFactory = new GithubSubmissionFactory(Context);
        var pullRequestCommentContextFactory = new PullRequestCommentContextFactory(githubPullRequestDescriptor, githubSubmissionFactory, Context, shreksCommandProcessor);
        return new ReviewWithDefenseGithubSubmissionStateMachine(
            Context,
            shreksCommandProcessor,
            new ShreksCommandProcessor(pullRequestCommentContextFactory, logger),
            logger,
            new TestEventNotifier());
    }
}