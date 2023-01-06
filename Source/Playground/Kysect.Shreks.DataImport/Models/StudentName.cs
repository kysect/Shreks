namespace Kysect.Shreks.DataImport;

public readonly record struct StudentName(string FirstName, string MiddleName, string LastName)
{
    public static StudentName FromString(string value)
    {
        string[] split = value.Trim()
            .Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

        string lastName = split.Length > 0 ? split[0] : string.Empty;
        string firstName = split.Length > 1 ? split[1] : string.Empty;
        string middleName = split.Length > 2 ? split[2] : string.Empty;

        return new StudentName(firstName, middleName, lastName);
    }
}