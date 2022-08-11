using FluentSpreadsheets;
using Kysect.Shreks.Core.Users;

namespace Kysect.Shreks.Integration.Google.Segments.Factories;

public interface IStudentComponentFactory
{
    IComponent BuildHeader();

    public IComponent BuildRow(Student student);
}