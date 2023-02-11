using ITMO.Dev.ASAP.Commands.SubmissionCommands;

namespace ITMO.Dev.ASAP.Commands.CommandVisitors;

public interface ISubmissionCommandVisitor
{
    Task VisitAsync(ActivateCommand command);

    Task VisitAsync(BanCommand command);

    Task VisitAsync(CreateSubmissionCommand command);

    Task VisitAsync(DeactivateCommand command);

    Task VisitAsync(DeleteCommand command);

    Task VisitAsync(HelpCommand command);

    Task VisitAsync(MarkReviewedCommand command);

    Task VisitAsync(RateCommand command);

    Task VisitAsync(UpdateCommand command);
}