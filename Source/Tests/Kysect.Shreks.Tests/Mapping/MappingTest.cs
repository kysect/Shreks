using AutoMapper;
using FluentAssertions;
using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Application.Dto.Users;
using Kysect.Shreks.Core.Models;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Core.Submissions;
using Kysect.Shreks.Core.UserAssociations;
using Kysect.Shreks.Core.Users;
using Kysect.Shreks.Seeding.EntityGenerators;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Kysect.Shreks.Tests.Mapping;

public class MappingTest : TestBase
{
    private readonly IMapper _mapper;

    public MappingTest()
    {
        _mapper = Provider.GetRequiredService<IMapper>();
    }

    [Fact]
    public void Map_Should_MapGithubSubmissionToSubmissionDto()
    {
        GithubSubmission submission = Provider.GetRequiredService<IEntityGenerator<GithubSubmission>>().Generate();

        SubmissionDto? submissionDto = _mapper.Map<SubmissionDto>(submission);

        Assert.NotNull(submissionDto);
    }

    [Theory]
    [InlineData(SubmissionStateDto.Active, SubmissionStateKind.Active)]
    [InlineData(SubmissionStateDto.Deleted, SubmissionStateKind.Deleted)]
    [InlineData(SubmissionStateDto.Inactive, SubmissionStateKind.Inactive)]
    [InlineData(SubmissionStateDto.Completed, SubmissionStateKind.Completed)]
    [InlineData(SubmissionStateDto.Reviewed, SubmissionStateKind.Reviewed)]
    [InlineData(SubmissionStateDto.Banned, SubmissionStateKind.Banned)]
    public void Map_Should_MapSubmissionStateDtoToSubmissionState(
        SubmissionStateDto stateDto,
        SubmissionStateKind state)
    {
        SubmissionStateKind receivedState = _mapper.Map<SubmissionStateKind>(stateDto);

        receivedState.Should().Be(state);
    }

    [Fact]
    public void Map_Should_MapStudentWithoutIsuToStudentDto()
    {
        Student student = Context.Students.First(x => x.User.Associations.OfType<IsuUserAssociation>().Any() == false);

        StudentDto? dto = _mapper.Map<StudentDto>(student);

        dto.Should().NotBeNull();
        dto.UniversityId.Should().BeNull();
    }

    [Fact]
    public void Map_Should_MapGroupAssignmentDtoToGroupAssignment()
    {
        GroupAssignment groupAssignment = Context.GroupAssignments.First();

        GroupAssignmentDto? groupAssignmentDto = _mapper.Map<GroupAssignmentDto>(groupAssignment);

        Assert.NotNull(groupAssignmentDto);
    }
}