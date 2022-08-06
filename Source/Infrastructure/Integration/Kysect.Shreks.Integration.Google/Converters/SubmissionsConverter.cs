using Kysect.Shreks.Core.Formatters;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Core.Users;

namespace Kysect.Shreks.Integration.Google.Converters;

public class SubmissionsConverter : ISheetRowConverter<Submission>
{
    private readonly IUserFullNameFormatter _userFullNameFormatter;

    public SubmissionsConverter(IUserFullNameFormatter userFullNameFormatter)
    {
        _userFullNameFormatter = userFullNameFormatter;
    }

    public IList<object> GetSheetRow(Submission submission)
    {
        Assignment assignment = submission.Assignment;
        Student student = submission.Student;

        return new List<object>
        {
            _userFullNameFormatter.GetFullName(student),
            student.Group.Name,
            //TODO: change to short name
            assignment.Title,
            submission.Payload
        };
    }
}