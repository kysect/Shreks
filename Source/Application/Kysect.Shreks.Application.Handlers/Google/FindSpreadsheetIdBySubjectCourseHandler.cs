﻿using Kysect.Shreks.Application.Abstractions.DataAccess;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Core.SubjectCourseAssociations;
using MediatR;
using static Kysect.Shreks.Application.Abstractions.Google.Queries.FindSpreadsheetIdBySubjectCourse;

namespace Kysect.Shreks.Application.Handlers.Google;

public class FindSpreadsheetIdBySubjectCourseHandler : IRequestHandler<Query, Response>
{
    private readonly IShreksDatabaseContext _context;

    public FindSpreadsheetIdBySubjectCourseHandler(IShreksDatabaseContext context)
    {
        _context = context;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        SubjectCourse course = await _context.SubjectCourses.GetByIdAsync(request.SubjectCourseId, cancellationToken);

        var googleTableAssociation = course.Associations
            .OfType<GoogleTableSubjectCourseAssociation>()
            .FirstOrDefault();

        return new Response(googleTableAssociation?.SpreadsheetId);
    }
}