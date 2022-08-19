using Kysect.Shreks.Application.Abstractions.DataAccess;
using Kysect.Shreks.Core.SubjectCourseAssociations;
using MediatR;
using static Kysect.Shreks.Application.Abstractions.Google.Commands.AddGoogleTableSubjectCourseAssociation;

namespace Kysect.Shreks.Application.Handlers.Google;

public class AddGoogleTableSubjectCourseAssociationHandler : IRequestHandler<Command, Unit>
{
    private readonly IShreksDatabaseContext _context;

    public AddGoogleTableSubjectCourseAssociationHandler(IShreksDatabaseContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
    {
        var subjectCourse = await _context.SubjectCourses.GetByIdAsync(request.SubjectCourseId, cancellationToken);

        var googleTableAssociation = new GoogleTableSubjectCourseAssociation(subjectCourse, request.SpreadsheetId);

        await _context.SubjectCourseAssociations.AddAsync(googleTableAssociation, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}