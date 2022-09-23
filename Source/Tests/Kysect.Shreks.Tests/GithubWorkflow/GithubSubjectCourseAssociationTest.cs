using FluentAssertions;
using Kysect.Shreks.Application.Commands.Processors;
using Kysect.Shreks.Application.DatabaseContextExtensions;
using Kysect.Shreks.Application.GithubWorkflow;
using Kysect.Shreks.Application.GithubWorkflow.Abstractions.Models;
using Kysect.Shreks.Application.GithubWorkflow.Submissions;
using Kysect.Shreks.Application.GithubWorkflow.SubmissionStateMachines;
using Kysect.Shreks.Application.TableManagement;
using Kysect.Shreks.Core.SubjectCourseAssociations;
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

    [Fact]
    public async Task PullRequestCreated_SubmissionShouldBeCreated()
    {
        (GithubSubjectCourseAssociation subjectCourseAssociation, Student student) = await TestContextGenerator.Create();
        GithubUserAssociation githubUserAssociation = student.User.Associations.OfType<GithubUserAssociation>().First();

        IReadOnlyCollection<GithubUserAssociation> githubUserAssociations = await Context
            .SubjectCourses
            .GetAllGithubUsers(subjectCourseAssociation.SubjectCourse.Id);

        IEnumerable<string> organizationUsers = githubUserAssociations.Select(a => a.GithubUsername);

        organizationUsers.Should().Contain(githubUserAssociation.GithubUsername);
    }

    public IGithubSubmissionStateMachine CreateSubmissionStateMachine(GithubPullRequestDescriptor githubPullRequestDescriptor)
    {
        var tableUpdateQueue = new TableUpdateQueue();
        ILogger logger = LogInitialization.GetLogger();

        var githubSubmissionFactory = new GithubSubmissionFactory(Context);
        var shreksCommandProcessor = new SubmissionService(Context, tableUpdateQueue);
        var pullRequestCommentContextFactory = new PullRequestCommentContextFactory(githubPullRequestDescriptor, githubSubmissionFactory, Context, shreksCommandProcessor);
        return new ReviewWithDefenseGithubSubmissionStateMachine(
            Context,
            shreksCommandProcessor,
            new ShreksCommandProcessor(pullRequestCommentContextFactory, logger),
            logger,
            new TestEventNotifier());
    }
}