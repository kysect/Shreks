using FluentSpreadsheets;
using Kysect.Shreks.Core.Formatters;
using Kysect.Shreks.Core.Users;
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
            Label("ФИО")
                .WithColumnWidth(240)
                .WithDefaultStyle(),
            Label("Группа").WithDefaultStyle()
        );
    }

    public IComponent BuildRow(Student student)
    {
        ArgumentNullException.ThrowIfNull(student);

        string studentName = _userFullNameFormatter.GetFullName(student.User);

        string groupName = student.Group.Name;

        return HStack
        (
            Label(studentName).WithDefaultStyle(),
            Label(groupName).WithDefaultStyle()
        );
    }
}