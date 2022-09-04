using FluentSpreadsheets;
using FluentSpreadsheets.Tables;
using Kysect.Shreks.Application.Abstractions.Formatters;
using Kysect.Shreks.Application.Dto.Tables;
using Kysect.Shreks.Integration.Google.Extensions;
using Kysect.Shreks.Integration.Google.Providers;
using static FluentSpreadsheets.ComponentFactory;

namespace Kysect.Shreks.Integration.Google.Tables;

public class QueueTable : RowTable<SubmissionsQueueDto>, ITableCustomizer
{
    private static readonly IRowComponent Header = Row
    (
        Label("ФИО").WithColumnWidth(240),
            Label("Группа"),
            Label("Лабораторная работа").WithColumnWidth(150),
            Label("Дата").WithColumnWidth(150),
            Label("GitHub").WithColumnWidth(400)
    );

    private readonly IUserFullNameFormatter _userFullNameFormatter;
    private readonly ICultureInfoProvider _cultureInfoProvider;

    public QueueTable(IUserFullNameFormatter userFullNameFormatter, ICultureInfoProvider cultureInfoProvider)
    {
        _userFullNameFormatter = userFullNameFormatter;
        _cultureInfoProvider = cultureInfoProvider;
    }

    public IComponent Customize(IComponent component)
        => component.WithDefaultStyle();

    protected override IEnumerable<IRowComponent> RenderRows(SubmissionsQueueDto queue)
    {
        yield return Header;

        foreach (var (student, submission) in queue.Submissions)
        {
            yield return Row
            (
                Label(_userFullNameFormatter.GetFullName(student.User)),
                Label(student.GroupName),
                Label(submission.AssignmentShortName),
                Label(submission.SubmissionDate, _cultureInfoProvider.GetCultureInfo()),
                Label(submission.Payload)
            );
        }
    }
}