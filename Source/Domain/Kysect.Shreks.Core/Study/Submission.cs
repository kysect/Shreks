using Kysect.Shreks.Core.Study.ValueObject;
using RichEntity.Annotations;

namespace Kysect.Shreks.Core.Study;

public partial class Submission : IEntity<Guid>
{
    public Submission(StudentAssignment studentAssignment)
        : this(Guid.NewGuid())
    {
        Rating = Rating.None;
        StudentAssignment = studentAssignment;
    }

    public virtual Rating Rating { get; set; }
    public virtual StudentAssignment StudentAssignment { get; protected init; }
}