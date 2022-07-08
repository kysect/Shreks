namespace Kysect.Shreks.Abstractions;

public record SubjectHomeworkInfo();
public record GroupQueue();

public record AddStudentSubmitArgument();
public record AddStudentToQueueArguments();


public interface IGoogleTableAccessor
{
    SubjectHomeworkInfo ReadSubjectHomeworkInfo();
    GroupQueue ReadGroupQueue();
    void AddStudentSubmit(AddStudentSubmitArgument arguments);
    void AddStudentToQueue(AddStudentToQueueArguments arguments);
}