using Kysect.Shreks.Core.Exceptions;
using Kysect.Shreks.Core.Users;
using RichEntity.Annotations;

namespace Kysect.Shreks.Core.Study;

public partial class StudentAssignment : IEntity
{
    private readonly HashSet<Submission> _submissions;

    public StudentAssignment(Student student, Assignment assignment)
        : this(studentId: student.Id, assignmentId: assignment.Id)
    {
        ArgumentNullException.ThrowIfNull(student);
        ArgumentNullException.ThrowIfNull(assignment);

        Student = student;
        Assignment = assignment;

        _submissions = new HashSet<Submission>();
    }

    [KeyProperty]
    public virtual Student Student { get; protected init; }

    [KeyProperty]
    public virtual Assignment Assignment { get; protected init; }
    
    public virtual IReadOnlyCollection<Submission> Submissions => _submissions;
    
    public void AddSubmission(Submission submission)
    {
        ArgumentNullException.ThrowIfNull(submission);

        if (!_submissions.Add(submission))
            throw new DomainInvalidOperationException($"Submission {submission} already exists");
    }
    
    public void RemoveSubmission(Submission submission)
    {
        ArgumentNullException.ThrowIfNull(submission);

        if (!_submissions.Remove(submission))
            throw new DomainInvalidOperationException($"Submission {submission} could not be removed");
    }
}