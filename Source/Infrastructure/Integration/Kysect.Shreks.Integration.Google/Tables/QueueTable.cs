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

public class QueueTable : RowTable<SubmissionsQueueDto>
{
    private static readonly IRowComponent Header = Row
    (
        Label("ФИО").WithColumnWidth(2).WithFrozenRows(),
        Label("Группа"),
        Label("Лабораторная работа").WithColumnWidth(1.2),
        Label("Дата").WithColumnWidth(1.2),
        Label("Статус"),
        Label("GitHub").WithColumnWidth(3.2).WithTrailingMediumBorder()
    );

    private readonly IUserFullNameFormatter _userFullNameFormatter;
    private readonly ICultureInfoProvider _cultureInfoProvider;

    public QueueTable(IUserFullNameFormatter userFullNameFormatter, ICultureInfoProvider cultureInfoProvider)
    {
        _userFullNameFormatter = userFullNameFormatter;
        _cultureInfoProvider = cultureInfoProvider;
    }

    protected override IComponent Customize(IComponent component)
    {
        return component.WithDefaultStyle();
    }

    protected override IEnumerable<IRowComponent> RenderRows(SubmissionsQueueDto model)
    {
        yield return Header;

        IReadOnlyList<QueueSubmissionDto> submissions = model.Submissions.ToArray();

        for (int i = 0; i < submissions.Count; i++)
        {
            (Application.Dto.Users.StudentDto student, SubmissionDto submission) = submissions[i];

            IRowComponent row = Row
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