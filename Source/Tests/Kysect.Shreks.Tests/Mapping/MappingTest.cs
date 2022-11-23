using AutoMapper;
using FluentAssertions;
using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Application.Dto.Users;
using Kysect.Shreks.Core.Models;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Core.Submissions;
using Kysect.Shreks.Core.Users;
using Kysect.Shreks.Seeding.EntityGenerators;
using Kysect.Shreks.Tests.DataAccess;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Kysect.Shreks.Tests.Mapping;

public class MappingTest : DataAccessTestBase
{
    private readonly IMapper _mapper;

    public MappingTest()
    {
        _mapper = Provider.GetRequiredService<IMapper>();
    }

    [Fact]
    public void Map_Should_MapGithubSubmissionToSubmissionDto()
    {
        var submission = Provider.GetRequiredService<IEntityGenerator<GithubSubmission>>().Generate();
        
        var submissionDto = _mapper.Map<SubmissionDto>(submission);

        Assert.NotNull(submissionDto);
    }

    [Theory]
    [InlineData(SubmissionStateDto.Active, SubmissionState.Active)]
    [InlineData(SubmissionStateDto.Deleted, SubmissionState.Deleted)]
    [InlineData(SubmissionStateDto.Inactive, SubmissionState.Inactive)]
    [InlineData(SubmissionStateDto.Completed, SubmissionState.Completed)]
    [InlineData(SubmissionStateDto.Reviewed, SubmissionState.Reviewed)]
    [InlineData(SubmissionStateDto.Banned, SubmissionState.Banned)]
    public void Map_Should_MapSubmissionStateDtoToSubmissionState(SubmissionStateDto stateDto, SubmissionState state)
    {
        var receivedState = _mapper.Map<SubmissionState>(stateDto);

        receivedState.Should().Be(state);
    }

    [Fact]
    public void Map_Should_MapStudentWithoutIsuToStudentDto()
    {
        var student = Provider.GetRequiredService<IEntityGenerator<Student>>().Generate();

        var dto = _mapper.Map<StudentDto>(student);

        dto.Should().NotBeNull();
        dto.UniversityId.Should().BeNull();
    }

    [Fact]
    public void Map_Should_MapGroupAssignmentDtoToGroupAssignment()
    {
        var groupAssignment = Provider.GetRequiredService<IEntityGenerator<GroupAssignment>>().Generate();

        var groupAssignmentDto = _mapper.Map<GroupAssignmentDto>(groupAssignment);

        Assert.NotNull(groupAssignmentDto);
    }
}