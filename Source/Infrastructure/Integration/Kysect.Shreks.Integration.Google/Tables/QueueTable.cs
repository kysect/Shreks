using System.Drawing;
using FluentSpreadsheets;
using FluentSpreadsheets.Tables;
using Kysect.Shreks.Application.Abstractions.Formatters;
using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Application.Dto.Tables;
using Kysect.Shreks.Integration.Google.Extensions;
using Kysect.Shreks.Integration.Google.Providers;
using static FluentSpreadsheets.ComponentFactory;

namespace Kysect.Shreks.Integration.Google.Tables;

public class QueueTable : RowTable<SubmissionsQueueDto>, ITableCustomizer
{
    private static readonly IRowComponent Header = Row
    (
        Label("ФИО").WithColumnWidth(240).WithFrozenRows(),
        Label("Группа"),
        Label("Лабораторная работа").WithColumnWidth(150),
        Label("Дата").WithColumnWidth(150),
        Label("Статус"),
        Label("GitHub").WithColumnWidth(400).WithTrailingMediumBorder()
    );

    private readonly IUserFullNameFormatter _userFullNameFormatter;
    private readonly ICultureInfoProvider _cultureInfoProvider;

    public QueueTable(IUserFullNameFormatter userFullNameFormatter, ICultureInfoProvider cultureInfoProvider)
    {
        _userFullNameFormatter = userFullNameFormatter;
        _cultureInfoProvider = cultureInfoProvider;
    }

    public IComponent Customize(IComponent component)
    {
        return component.WithDefaultStyle();
    }

    protected override IEnumerable<IRowComponent> RenderRows(SubmissionsQueueDto queue)
    {
        yield return Header;

        IReadOnlyList<QueueSubmissionDto> submissions = queue.Submissions.ToArray();

        for (int i = 0; i < submissions.Count; i++)
        {
            var(student, submission) = submissions[i];

            var row = Row
            (
                Label(_userFullNameFormatter.GetFullName(student.User)),
                Label(student.GroupName),
                Label(submission.AssignmentShortName),
                Label(submission.SubmissionDate, _cultureInfoProvider.GetCultureInfo()),
                Label(submission.State.ToString()),
                Label(submission.Payload).WithTrailingMediumBorder()
            ).WithDefaultStyle(i, submissions.Count);

            if (submission.State is SubmissionStateDto.Reviewed)
                row = row.FilledWith(125, Color.LightGreen);

            yield return row;
        }
    }
}