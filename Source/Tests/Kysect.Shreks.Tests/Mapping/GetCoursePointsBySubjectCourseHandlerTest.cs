using AutoMapper;
using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Seeding.EntityGenerators;
using Kysect.Shreks.Tests.DataAccess;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Kysect.Shreks.Tests.Mapping;

public class MappingTest : DataAccessTestBase
{
    private readonly IEntityGenerator<Submission> _submissionGenerator;
    private IMapper _mapper;

    public MappingTest()
    {
        _submissionGenerator = Provider.GetRequiredService<IEntityGenerator<Submission>>();
        _mapper = Provider.GetRequiredService<IMapper>();
    }

    [Fact]
    public async Task MapSubmissionToDto_NoException()
    {
        Submission submission = _submissionGenerator.Generate();
        var submissionDto = _mapper.Map<SubmissionDto>(submission);

        Assert.NotNull(submissionDto);
    }
}