using Kysect.Shreks.Core.Models;
using Kysect.Shreks.Core.Submissions.States;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Kysect.Shreks.DataAccess.ValueConverters;

public class SubmissionStateValueConverter : ValueConverter<ISubmissionState, SubmissionStateKind>
{
    public SubmissionStateValueConverter()
        : base(x => ConvertTo(x), x => ConvertFrom(x)) { }

    private static SubmissionStateKind ConvertTo(ISubmissionState state)
    {
        return state.Kind;
    }

    private static ISubmissionState ConvertFrom(SubmissionStateKind kind)
    {
        return kind switch
        {
            SubmissionStateKind.Active => new ActiveSubmissionState(),
            SubmissionStateKind.Inactive => new InactiveSubmissionState(),
            SubmissionStateKind.Deleted => new DeletedSubmissionState(),
            SubmissionStateKind.Completed => new CompletedSubmissionState(),
            SubmissionStateKind.Reviewed => new ReviewedSubmissionState(),
            SubmissionStateKind.Banned => new BannedSubmissionState(),
            _ => throw new ArgumentOutOfRangeException(nameof(kind), kind, null),
        };
    }
}