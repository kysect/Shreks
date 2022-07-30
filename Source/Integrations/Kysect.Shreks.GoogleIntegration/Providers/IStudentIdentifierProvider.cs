using Kysect.Shreks.Core.Users;

namespace Kysect.Shreks.GoogleIntegration.Providers;

public interface IStudentIdentifierProvider
{
    string GetStudentIdentifier(Student student);
}