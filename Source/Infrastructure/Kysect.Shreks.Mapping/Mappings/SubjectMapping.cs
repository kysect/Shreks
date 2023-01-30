using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Core.Study;

namespace Kysect.Shreks.Mapping.Mappings;

public static class SubjectMapping
{
    public static SubjectDto ToDto(this Subject subject)
    {
        return new SubjectDto(subject.Id, subject.Title);
    }
}