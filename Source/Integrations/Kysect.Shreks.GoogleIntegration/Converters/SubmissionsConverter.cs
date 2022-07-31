using Kysect.Shreks.Core.Formatters;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Core.Users;

namespace Kysect.Shreks.GoogleIntegration.Converters;

public class SubmissionsConverter : ISheetRowConverter<Submission>
{
    private readonly IUserFullNameFormatter _userFullNameFormatter;

    public SubmissionsConverter(IUserFullNameFormatter userFullNameFormatter)
    {
        _userFullNameFormatter = userFullNameFormatter;
    }

    public IList<object> GetSheetRow(Submission submission)
    {
        StudentAssignment assignment = submission.StudentAssignment;
        Student student = assignment.Student;

        return new List<object>
        {
            _userFullNameFormatter.GetFullName(student),
            student.Group.Name,
            //TODO: change to short name
            assignment.Assignment.Title,
            //TODO: change to real pr link
            "https://github.com/kysect/Shreks/pull/29"
        };
    }
}