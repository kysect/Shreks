using FluentSpreadsheets;
using FluentSpreadsheets.Tables;
using ITMO.Dev.ASAP.Application.Abstractions.Formatters;
using ITMO.Dev.ASAP.Application.Dto.Study;
using ITMO.Dev.ASAP.Application.Dto.Tables;
using ITMO.Dev.ASAP.Application.Dto.Users;
using ITMO.Dev.ASAP.Integration.Google.Extensions;
using ITMO.Dev.ASAP.Integration.Google.Providers;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using static FluentSpreadsheets.ComponentFactory;

namespace ITMO.Dev.ASAP.Integration.Google.Tables;

public class QueueTable : RowTable<SubmissionsQueueDto>
{
    private static readonly IRowComponent Header = Row(
        Label("ФИО").WithColumnWidth(2).WithFrozenRows(),
        Label("Группа"),
        Label("Лабораторная работа").WithColumnWidth(1.2),
        Label("Дата").WithColumnWidth(1.2),
        Label("Статус"),
        Label("GitHub").WithColumnWidth(3.2).WithTrailingMediumBorder());

    private readonly ICultureInfoProvider _cultureInfoProvider;

    private readonly IUserFullNameFormatter _userFullNameFormatter;

    public QueueTable(IUserFullNameFormatter userFullNameFormatter, ICultureInfoProvider cultureInfoProvider)
    {
        _userFullNameFormatter = userFullNameFormatter;
        _cultureInfoProvider = cultureInfoProvider;
    }

    protected override IComponent Customize(IComponent component)
    {
        return component.WithDefaultStyle();
    }

    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1110:Opening parenthesis or bracket should be on declaration line", Justification = "FluentSpreadsheets components setup readability is messy with this rule applied")]
    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1111:Closing parenthesis should be on line of last parameter", Justification = "FluentSpreadsheets components setup readability is messy with this rule applied")]
    [SuppressMessage("StyleCop.CSharp.SpacingRules", "SA1009:Closing parenthesis should be spaced correctly", Justification = "FluentSpreadsheets components setup readability is messy with this rule applied")]
    protected override IEnumerable<IRowComponent> RenderRows(SubmissionsQueueDto model)
    {
        yield return Header;

        IReadOnlyList<QueueSubmissionDto> submissions = model.Submissions;

        for (int i = 0; i < submissions.Count; i++)
        {
            (StudentDto student, SubmissionDto submission) = submissions[i];

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