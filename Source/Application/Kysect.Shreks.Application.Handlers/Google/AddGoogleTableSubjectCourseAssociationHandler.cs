using Kysect.Shreks.Application.Abstractions.DataAccess;
using Kysect.Shreks.Core.SubjectCourseAssociations;
using MediatR;
using static Kysect.Shreks.Application.Abstractions.Google.Queries.AddGoogleTableSubjectCourseAssociation;

namespace Kysect.Shreks.Application.Handlers.Google;

public class AddGoogleTableSubjectCourseAssociationHandler : IRequestHandler<Query, Unit>
{
    private readonly IShreksDatabaseContext _context;

    public AddGoogleTableSubjectCourseAssociationHandler(IShreksDatabaseContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(Query request, CancellationToken cancellationToken)
    {
        var subjectCourse = await _context.SubjectCourses.GetByIdAsync(request.SubjectCourseId, cancellationToken);

        var googleTableAssociation = new GoogleTableSubjectCourseAssociation(subjectCourse, request.SpreadsheetId);

        await _context.SubjectCourseAssociations.AddAsync(googleTableAssociation, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}