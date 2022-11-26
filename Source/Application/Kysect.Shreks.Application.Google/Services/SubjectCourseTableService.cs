using Kysect.Shreks.Application.Abstractions.Google;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Core.SubjectCourseAssociations;
using Kysect.Shreks.DataAccess.Abstractions;
using Kysect.Shreks.DataAccess.Abstractions.Extensions;
using Kysect.Shreks.Integration.Google.Tools;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace Kysect.Shreks.Application.Google.Services;

public class SubjectCourseTableService : ISubjectCourseTableService
{
    private readonly IShreksDatabaseContext _context;
    private readonly ISpreadsheetManagementService _spreadsheetManagementService;
    private readonly ConcurrentDictionary<Guid, Task<string>> _tableIdCache;
    private readonly ILogger<SubjectCourseTableService> _logger;

    public SubjectCourseTableService(
        IShreksDatabaseContext context,
        ISpreadsheetManagementService spreadsheetManagementService,
        ILogger<SubjectCourseTableService> logger)
    {
        _context = context;
        _spreadsheetManagementService = spreadsheetManagementService;
        _logger = logger;
        _tableIdCache = new ConcurrentDictionary<Guid, Task<string>>();
    }

    public Task<string> GetSubjectCourseTableId(Guid subjectCourseId, CancellationToken cancellationToken)
    {
        return _tableIdCache.GetOrAdd(subjectCourseId, async id =>
        {
            GoogleTableSubjectCourseAssociation? spreadsheetAssociation = await _context.SubjectCourseAssociations
                .Where(x => x.SubjectCourse.Id.Equals(id))
                .OfType<GoogleTableSubjectCourseAssociation>()
                .SingleOrDefaultAsync(cancellationToken);

            if (spreadsheetAssociation is not null)
                return spreadsheetAssociation.SpreadsheetId;

            _logger.LogInformation("Spreadsheet of course {SubjectCourseId} was not found and will be created.",
                subjectCourseId);

            SubjectCourse subjectCourse = await _context.SubjectCourses.GetByIdAsync(id, cancellationToken);

            string spreadsheetId = await _spreadsheetManagementService
                .CreateSpreadsheetAsync(subjectCourse.Title, cancellationToken);

            spreadsheetAssociation = new GoogleTableSubjectCourseAssociation(subjectCourse, spreadsheetId);
            _context.SubjectCourseAssociations.Add(spreadsheetAssociation);

            await _context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Successfully created spreadsheet of course {SubjectCourseId}.", subjectCourseId);

            _tableIdCache.TryRemove(id, out _);
            return spreadsheetId;
        });
    }
}