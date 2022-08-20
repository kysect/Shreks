using FluentSpreadsheets;
using FluentSpreadsheets.SheetSegments;
using Kysect.Shreks.Core.Exceptions;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Core.SubmissionAssociations;
using Kysect.Shreks.Integration.Google.Extensions;
using MediatR;
using static FluentSpreadsheets.ComponentFactory;

namespace Kysect.Shreks.Integration.Google.Segments;

public class AssignmentDataSegment : SheetSegmentBase<Unit, Submission, Unit>
{
    protected override IComponent BuildHeader(Unit data)
    {
        return HStack
        (
            Label("Лабораторная работа")
                .WithColumnWidth(150)
                .WithDefaultStyle(),
            Label("GitHub")
                .WithColumnWidth(400)
                .WithDefaultStyle()
        );
    }

    protected override IComponent BuildRow(HeaderRowData<Unit, Submission> data, int rowIndex)
    {
        Submission submission = data.RowData;

        ArgumentNullException.ThrowIfNull(submission.Association);
        if (submission.Association is not GithubPullRequestSubmissionAssociation pullRequest)
            throw new DomainInvalidOperationException($"Cannot build google sheet row. Submission has invalid association: {submission.Association.GetType()}");

        // TODO: use full link to PR instead of number
        return HStack
        (
            Label(submission.Assignment.ShortName).WithDefaultStyle(),
            Label(pullRequest.PullRequestNumber).WithDefaultStyle()
        );
    }
}