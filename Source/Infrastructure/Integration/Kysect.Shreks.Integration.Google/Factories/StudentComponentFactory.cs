using FluentSpreadsheets;
using Kysect.Shreks.Application.Abstractions.Formatters;
using Kysect.Shreks.Application.Dto.Users;
using Kysect.Shreks.Integration.Google.Extensions;
using static FluentSpreadsheets.ComponentFactory;

namespace Kysect.Shreks.Integration.Google.Factories;

public class StudentComponentFactory : IStudentComponentFactory
{
    private readonly IUserFullNameFormatter _userFullNameFormatter;

    public StudentComponentFactory(IUserFullNameFormatter userFullNameFormatter)
    {
        _userFullNameFormatter = userFullNameFormatter;
    }

    public IComponent BuildHeader()
    {
        return HStack
        (
            Label("ФИО").WithColumnWidth(240),
            Label("Группа")
        ).WithDefaultStyle();
    }

    public IComponent BuildRow(StudentDto student)
    {
        ArgumentNullException.ThrowIfNull(student);

        var studentName = _userFullNameFormatter.GetFullName(student.User);
        var groupName = student.GroupName;

        return HStack
        (
            Label(studentName),
            Label(groupName)
        ).WithDefaultStyle();
    }
}