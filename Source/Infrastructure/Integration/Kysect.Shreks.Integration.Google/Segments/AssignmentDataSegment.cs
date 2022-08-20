using FluentSpreadsheets;
using FluentSpreadsheets.SheetSegments;
using Kysect.Shreks.Core.Exceptions;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Core.SubmissionAssociations;
using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Application.Dto.Tables;
using Kysect.Shreks.Integration.Google.Extensions;
using MediatR;
using static FluentSpreadsheets.ComponentFactory;

namespace Kysect.Shreks.Integration.Google.Segments;

public class AssignmentDataSegment : SheetSegmentBase<Unit, QueueSubmissionDto, Unit>
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

    protected override IComponent BuildRow(HeaderRowData<Unit, QueueSubmissionDto> data, int rowIndex)
    {
        var submission = data.RowData.Submission;

        ArgumentNullException.ThrowIfNull(submission.Association);
        if (submission.Association is not GithubPullRequestSubmissionAssociation pullRequest)
            throw new DomainInvalidOperationException($"Cannot build google sheet row. Submission has invalid association: {submission.Association.GetType()}");

        // TODO: use full link to PR instead of number
        return HStack
        (
            Label(submission.AssignmentShortName).WithDefaultStyle(),
            Label(submission.Payload).WithDefaultStyle()
        );
    }
}