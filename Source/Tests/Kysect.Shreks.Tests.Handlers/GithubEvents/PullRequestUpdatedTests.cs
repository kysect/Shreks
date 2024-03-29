using FluentAssertions;
using Kysect.Shreks.Application.Abstractions.Submissions;
using Kysect.Shreks.Application.Contracts.GithubEvents;
using Kysect.Shreks.Application.Handlers.GithubEvents;
using Kysect.Shreks.Common.Exceptions;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Core.Users;
using Kysect.Shreks.Tests.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Kysect.Shreks.Tests.Handlers.GithubEvents;

public class PullRequestUpdatedTests : GithubEventsTestBase
{
    [Fact]
    public async Task Handle_ShouldCreateSubmission_WhenStudentHadNoSubmissionOnAssignmentAndIssuedByStudent()
    {
        GroupAssignment groupAssignment = await Context.GetGroupAssignmentWithUnSubmittedStudents();
        Student student = groupAssignment.GetUnSubmittedStudent();
        string organizationName = groupAssignment.GetOrganizationName();
        string repositoryName = student.GetRepositoryName();

        ISubmissionWorkflowService workflowService = Provider.GetRequiredService<ISubmissionWorkflowService>();

        var command = new PullRequestUpdated.Command(
            student.UserId,
            student.UserId,
            groupAssignment.AssignmentId,
            organizationName,
            repositoryName,
            1,
            "payload");

        var handler = new PullRequestUpdatedHandler(Context, workflowService);

        PullRequestUpdated.Response response = await handler.Handle(command, CancellationToken.None);

        response.Message.IsCreated.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ShouldCreateSubmissionWhenStudentHadNotSubmissionOnAssignmentAndIssuedByMentor()
    {
        GroupAssignment groupAssignment = await Context.GetGroupAssignmentWithUnSubmittedStudents();
        Student student = groupAssignment.GetUnSubmittedStudent();
        Mentor mentor = groupAssignment.GetMentor();

        string organizationName = groupAssignment.GetOrganizationName();
        string repositoryName = student.GetRepositoryName();

        ISubmissionWorkflowService workflowService = Provider.GetRequiredService<ISubmissionWorkflowService>();

        var command = new PullRequestUpdated.Command(
            mentor.UserId,
            student.UserId,
            groupAssignment.AssignmentId,
            organizationName,
            repositoryName,
            1,
            "payload");

        var handler = new PullRequestUpdatedHandler(Context, workflowService);

        PullRequestUpdated.Response response = await handler.Handle(command, CancellationToken.None);

        response.Message.IsCreated.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ShouldThrowWhenStudentHadNoSubmissionOnAssignmentAndIssuedByForeignEntity()
    {
        GroupAssignment groupAssignment = await Context.GetGroupAssignmentWithUnSubmittedStudents();
        Student student = groupAssignment.GetUnSubmittedStudent();
        User issuer = await Context.GetNotMentorUser(groupAssignment, student.User);

        string organizationName = groupAssignment.GetOrganizationName();
        string repositoryName = student.GetRepositoryName();

        ISubmissionWorkflowService workflowService = Provider.GetRequiredService<ISubmissionWorkflowService>();

        var command = new PullRequestUpdated.Command(
            issuer.Id,
            student.UserId,
            groupAssignment.AssignmentId,
            organizationName,
            repositoryName,
            1,
            "payload");

        var handler = new PullRequestUpdatedHandler(Context, workflowService);

        Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<UnauthorizedException>();
    }

    [Fact]
    public async Task Handle_ShouldUpdateSubmission_WhenStudentHadSubmissionOnAssignmentAndIssuedByStudent()
    {
        GroupAssignment groupAssignment = await Context.GetGroupAssignmentWithSubmittedStudents();
        Student student = groupAssignment.GetSubmittedStudent();
        string organizationName = groupAssignment.GetOrganizationName();
        string repositoryName = student.GetRepositoryName();

        ISubmissionWorkflowService workflowService = Provider.GetRequiredService<ISubmissionWorkflowService>();

        var command = new PullRequestUpdated.Command(
            student.UserId,
            student.UserId,
            groupAssignment.AssignmentId,
            organizationName,
            repositoryName,
            1,
            "payload");

        var handler = new PullRequestUpdatedHandler(Context, workflowService);

        PullRequestUpdated.Response response = await handler.Handle(command, CancellationToken.None);

        response.Message.IsCreated.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_ShouldUpdateSubmissionWhenStudentHadSubmissionOnAssignmentAndIssuedByMentor()
    {
        GroupAssignment groupAssignment = await Context.GetGroupAssignmentWithSubmittedStudents();
        Student student = groupAssignment.GetSubmittedStudent();
        Mentor mentor = groupAssignment.GetMentor();

        string organizationName = groupAssignment.GetOrganizationName();
        string repositoryName = student.GetRepositoryName();

        ISubmissionWorkflowService workflowService = Provider.GetRequiredService<ISubmissionWorkflowService>();

        var command = new PullRequestUpdated.Command(
            mentor.UserId,
            student.UserId,
            groupAssignment.AssignmentId,
            organizationName,
            repositoryName,
            1,
            "payload");

        var handler = new PullRequestUpdatedHandler(Context, workflowService);

        PullRequestUpdated.Response response = await handler.Handle(command, CancellationToken.None);

        response.Message.IsCreated.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_ShouldThrowWhenStudentHadSubmissionOnAssignmentAndIssuedByForeignEntity()
    {
        GroupAssignment groupAssignment = await Context.GetGroupAssignmentWithSubmittedStudents();
        Student student = groupAssignment.GetSubmittedStudent();
        User issuer = await Context.GetNotMentorUser(groupAssignment, student.User);

        string organizationName = groupAssignment.GetOrganizationName();
        string repositoryName = student.GetRepositoryName();

        ISubmissionWorkflowService workflowService = Provider.GetRequiredService<ISubmissionWorkflowService>();

        var command = new PullRequestUpdated.Command(
            issuer.Id,
            student.UserId,
            groupAssignment.AssignmentId,
            organizationName,
            repositoryName,
            1,
            "payload");

        var handler = new PullRequestUpdatedHandler(Context, workflowService);

        Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<UnauthorizedException>();
    }
}