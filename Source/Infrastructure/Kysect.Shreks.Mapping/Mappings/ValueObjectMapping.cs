using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Core.Models;
using Kysect.Shreks.Core.SubmissionStateWorkflows;
using Kysect.Shreks.Core.Tools;

namespace Kysect.Shreks.Mapping.Mappings;

public static class ValueObjectMapping
{
    public static DateTime AsDateTime(this DateOnly date)
        => date.ToDateTime(new TimeOnly(0));

    public static DateOnly AsDateOnly(this DateTime date)
        => DateOnly.FromDateTime(date);

    public static DateTime AsDateTime(this SpbDateTime date)
        => date.Value;

    public static SpbDateTime AsSpbDateTime(this DateTime date)
        => new SpbDateTime(date);

    public static SubmissionStateWorkflowTypeDto AsDto(this SubmissionStateWorkflowType type)
    {
        return type switch
        {
            SubmissionStateWorkflowType.ReviewOnly => SubmissionStateWorkflowTypeDto.ReviewOnly,
            SubmissionStateWorkflowType.ReviewWithDefense => SubmissionStateWorkflowTypeDto.ReviewWithDefense,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null),
        };
    }

    public static SubmissionStateWorkflowType AsValueObject(this SubmissionStateWorkflowTypeDto dto)
    {
        return dto switch
        {
            SubmissionStateWorkflowTypeDto.ReviewOnly => SubmissionStateWorkflowType.ReviewOnly,
            SubmissionStateWorkflowTypeDto.ReviewWithDefense => SubmissionStateWorkflowType.ReviewWithDefense,
            _ => throw new ArgumentOutOfRangeException(nameof(dto), dto, null),
        };
    }

    public static SubmissionStateDto AsDto(this SubmissionStateKind stateKind)
    {
        return stateKind switch
        {
            SubmissionStateKind.Active => SubmissionStateDto.Active,
            SubmissionStateKind.Inactive => SubmissionStateDto.Inactive,
            SubmissionStateKind.Deleted => SubmissionStateDto.Deleted,
            SubmissionStateKind.Completed => SubmissionStateDto.Completed,
            SubmissionStateKind.Reviewed => SubmissionStateDto.Reviewed,
            SubmissionStateKind.Banned => SubmissionStateDto.Banned,
            _ => throw new ArgumentOutOfRangeException(nameof(stateKind), stateKind, null),
        };
    }
}