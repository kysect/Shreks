using FluentSpreadsheets;
using Kysect.Shreks.Application.Dto.Users;

namespace Kysect.Shreks.Integration.Google.Factories;

public interface IStudentComponentFactory
{
    IComponent BuildHeader();
    
    IComponent BuildRow(StudentDto student);
}