using FluentSpreadsheets;
using Kysect.Shreks.Core.Users;

namespace Kysect.Shreks.Integration.Google.Factories;

public interface IStudentComponentFactory
{
    IComponent BuildHeader();

   IComponent BuildRow(Student student);
}