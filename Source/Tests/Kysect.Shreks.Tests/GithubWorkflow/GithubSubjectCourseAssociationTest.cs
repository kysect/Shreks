// using FluentAssertions;
// using Kysect.Shreks.Application.DatabaseContextExtensions;
// using Kysect.Shreks.Application.GithubWorkflow.Abstractions.Models;
// using Kysect.Shreks.Common.Exceptions;
// using Kysect.Shreks.Core.Models;
// using Kysect.Shreks.Core.Submissions;
// using Kysect.Shreks.Core.Tools;
// using Kysect.Shreks.Core.UserAssociations;
// using Kysect.Shreks.Tests.GithubWorkflow.Tools;
// using Kysect.Shreks.Tests.Tools;
// using Microsoft.Extensions.Logging;
// using Xunit;
// using Xunit.Abstractions;
//
// namespace Kysect.Shreks.Tests.GithubWorkflow;
//
// [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores",
//     Justification = "<Pending>")]
// public class GithubSubjectCourseAssociationTest : GithubWorkflowTestBase
// {
//     private readonly ILogger _logger;
//     private readonly TestEventNotifier _pullRequestCommitEventNotifier;
//
//     public GithubSubjectCourseAssociationTest(ITestOutputHelper output)
//     {
//         _logger = LogInitialization.InitTestLogger(output);
//
//         _pullRequestCommitEventNotifier = new TestEventNotifier();
//     }
//
//     [Fact]
//     public async Task GetSubjectCourseGithubUser_StudentAssociationExists_AssociationShouldReturn()
//     {
//         GithubApplicationTestContext githubApplicationTestContext = await TestContextGenerator.Create();
//         GithubUserAssociation githubUserAssociation = githubApplicationTestContext.GetStudentGithubAssociation();
//
//         IReadOnlyCollection<GithubUserAssociation> githubUserAssociations = await Context
//             .SubjectCourses
//             .GetAllGithubUsers(githubApplicationTestContext.SubjectCourseAssociation.SubjectCourse.Id);
//
//         IEnumerable<string> organizationUsers = githubUserAssociations.Select(a => a.GithubUsername);
//
//         organizationUsers.Should().Contain(githubUserAssociation.GithubUsername);
//     }
//
//     [Fact]
//     public async Task PullRequestCreated_SubmissionShouldBeCreated()
//     {
//         GithubApplicationTestContext context = await TestContextGenerator.Create();
//         GithubPullRequestDescriptor githubPullRequestDescriptor = context.CreateStudentPullRequestDescriptor();
//
//         await _githubSubmissionStateMachine.ProcessPullRequestUpdate(githubPullRequestDescriptor, _logger,
//             _pullRequestCommitEventNotifier, CancellationToken.None);
//         Submission lastSubmissionByPr =
//             await _githubSubmissionService.GetLastSubmissionByPr(githubPullRequestDescriptor);
//
//         Assert.Equal(SubmissionStateKind.Active, lastSubmissionByPr.State.Kind);
//         Assert.Single(_pullRequestCommitEventNotifier.PullRequestMessages);
//     }
//
//     [Fact]
//     public async Task PullRequestUpdate_AfterSubmissionWasCreated_SubmissionShouldUpdateTime()
//     {
//         GithubApplicationTestContext context = await TestContextGenerator.Create();
//         GithubPullRequestDescriptor githubPullRequestDescriptor = context.CreateStudentPullRequestDescriptor();
//
//         await _githubSubmissionStateMachine.ProcessPullRequestUpdate(githubPullRequestDescriptor, _logger,
//             _pullRequestCommitEventNotifier, CancellationToken.None);
//         Submission createdSubmission = await _githubSubmissionService.GetLastSubmissionByPr(githubPullRequestDescriptor);
//         // Do not inline, it lead to test failing
//         SpbDateTime submissionCreatedTime = createdSubmission.SubmissionDate;
//
//         await _githubSubmissionStateMachine.ProcessPullRequestUpdate(githubPullRequestDescriptor, _logger,
//             _pullRequestCommitEventNotifier, CancellationToken.None);
//         Submission updatedSubmission = await _githubSubmissionService.GetLastSubmissionByPr(githubPullRequestDescriptor);
//
//         Assert.Equal(updatedSubmission.Id, createdSubmission.Id);
//         Assert.NotEqual(updatedSubmission.SubmissionDate, submissionCreatedTime);
//         Assert.Single(_pullRequestCommitEventNotifier.PullRequestMessages);
//     }
//
//     [Fact]
//     public async Task ProcessPullRequestClosed_WithoutMerge_SubmissionShouldBeInactive()
//     {
//         GithubApplicationTestContext context = await TestContextGenerator.Create();
//         GithubPullRequestDescriptor githubPullRequestDescriptor = context.CreateStudentPullRequestDescriptor();
//
//         await _githubSubmissionStateMachine.ProcessPullRequestUpdate(githubPullRequestDescriptor, _logger,
//             _pullRequestCommitEventNotifier, CancellationToken.None);
//         await _githubSubmissionStateMachine.ProcessPullRequestClosed(merged: false, githubPullRequestDescriptor, _logger,
//             _pullRequestCommitEventNotifier);
//
//         Submission submission = await _githubSubmissionService.GetLastSubmissionByPr(githubPullRequestDescriptor);
//
//         Assert.Equal(SubmissionStateKind.Inactive, submission.State.Kind);
//     }
//
//     [Fact]
//     public async Task ProcessPullRequestClosed_WithMerge_SubmissionShouldBeCompleted()
//     {
//         GithubApplicationTestContext context = await TestContextGenerator.Create();
//         GithubPullRequestDescriptor githubPullRequestDescriptor = context.CreateStudentPullRequestDescriptor();
//
//         await _githubSubmissionStateMachine.ProcessPullRequestUpdate(githubPullRequestDescriptor, _logger,
//             _pullRequestCommitEventNotifier, CancellationToken.None);
//         await _githubSubmissionStateMachine.ProcessPullRequestClosed(merged: true, githubPullRequestDescriptor, _logger,
//             _pullRequestCommitEventNotifier);
//
//         Submission submission = await _githubSubmissionService.GetLastSubmissionByPr(githubPullRequestDescriptor);
//
//         Assert.Equal(SubmissionStateKind.Completed, submission.State.Kind);
//     }
//
//     [Fact]
//     public async Task ProcessPullRequestReopen_AfterClose_SubmissionShouldBeActive()
//     {
//         GithubApplicationTestContext context = await TestContextGenerator.Create();
//         GithubPullRequestDescriptor githubPullRequestDescriptor = context.CreateStudentPullRequestDescriptor();
//
//         await _githubSubmissionStateMachine.ProcessPullRequestUpdate(githubPullRequestDescriptor, _logger,
//             _pullRequestCommitEventNotifier, CancellationToken.None);
//         await _githubSubmissionStateMachine.ProcessPullRequestClosed(merged: false, githubPullRequestDescriptor, _logger,
//             _pullRequestCommitEventNotifier);
//         await _githubSubmissionStateMachine.ProcessPullRequestReopen(githubPullRequestDescriptor, _logger,
//             _pullRequestCommitEventNotifier);
//
//         Submission submission = await _githubSubmissionService.GetLastSubmissionByPr(githubPullRequestDescriptor);
//
//         Assert.Equal(SubmissionStateKind.Active, submission.State.Kind);
//     }
//
//     [Fact]
//     public async Task ProcessPullRequestReviewApprove_ByStudent_ActionShouldBeForbidden()
//     {
//         GithubApplicationTestContext context = await TestContextGenerator.Create();
//         GithubPullRequestDescriptor githubPullRequestDescriptor = context.CreateStudentPullRequestDescriptor();
//
//         await _githubSubmissionStateMachine.ProcessPullRequestUpdate(
//             githubPullRequestDescriptor, _logger, _pullRequestCommitEventNotifier, CancellationToken.None);
//
//         await Assert.ThrowsAsync<UnauthorizedException>(async () =>
//         {
//             await _githubSubmissionStateMachine.ProcessPullRequestReviewApprove(
//                 null, githubPullRequestDescriptor, _logger, _pullRequestCommitEventNotifier);
//         });
//
//         Submission submission = await _githubSubmissionService.GetLastSubmissionByPr(githubPullRequestDescriptor);
//
//         Assert.Null(submission.Rating);
//         Assert.Equal(SubmissionStateKind.Active, submission.State.Kind);
//     }
// }