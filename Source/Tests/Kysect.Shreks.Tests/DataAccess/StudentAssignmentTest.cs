using FluentAssertions;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Seeding.EntityGenerators;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Kysect.Shreks.Tests.DataAccess;

public class StudentAssignmentTest : DataAccessTestBase
{
    private readonly IEntityGenerator<StudentAssignment> _studentAssignmentGenerator;

    public StudentAssignmentTest()
    {
        _studentAssignmentGenerator = Provider.GetRequiredService<IEntityGenerator<StudentAssignment>>();
    }
}