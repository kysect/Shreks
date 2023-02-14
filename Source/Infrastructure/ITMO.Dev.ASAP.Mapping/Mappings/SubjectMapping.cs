using ITMO.Dev.ASAP.Application.Dto.Study;
using ITMO.Dev.ASAP.Core.Study;

namespace ITMO.Dev.ASAP.Mapping.Mappings;

public static class SubjectMapping
{
    public static SubjectDto ToDto(this Subject subject)
    {
        return new SubjectDto(subject.Id, subject.Title);
    }
}