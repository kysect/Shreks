using Ardalis.Result;
using Kysect.Shreks.Core.Users;
using RichEntity.Annotations;

namespace Kysect.Shreks.Core.Study;

public partial class StudentAssignment : IEntity
{
    private readonly List<Submission> _submissions;

    public StudentAssignment(Student student, Assignment assignment)
        : this(studentId: student.Id, assignmentId: assignment.Id)
    {
        ArgumentNullException.ThrowIfNull(student);
        ArgumentNullException.ThrowIfNull(assignment);

        Student = student;
        Assignment = assignment;

        _submissions = new List<Submission>();
    }

    [KeyProperty]
    public virtual Student Student { get; protected init; }

    [KeyProperty]
    public virtual Assignment Assignment { get; protected init; }
    
    public IReadOnlyCollection<Submission> Submissions => _submissions.AsReadOnly();
    
    public Result AddSubmission(Submission submission)
    {
        ArgumentNullException.ThrowIfNull(submission);
        
        if (_submissions.Contains(submission))
            return Result.Error($"Submission {submission} already exists");
        
        _submissions.Add(submission);
        return Result.Success();
    }
    
    public Result RemoveSubmission(Submission submission)
    {
        ArgumentNullException.ThrowIfNull(submission);
        
        if (!_submissions.Contains(submission))
            return Result.Error($"Submission {submission} does not exist");
        
        if (!_submissions.Remove(submission))
            return Result.Error($"Submission {submission} could not be removed");
        
        return Result.Success();
    }
}