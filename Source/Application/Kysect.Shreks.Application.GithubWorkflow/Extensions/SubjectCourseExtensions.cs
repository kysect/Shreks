using Kysect.Shreks.Common.Exceptions;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Core.SubjectCourseAssociations;
using Microsoft.EntityFrameworkCore;

namespace Kysect.Shreks.Application.GithubWorkflow.Extensions;

public static class SubjectCourseExtensions
{
    public static async Task<SubjectCourse> GetSubjectCourseByOrganization(
        this DbSet<SubjectCourseAssociation> associations,
        string organization,
        CancellationToken cancellationToken)
    {
        GithubSubjectCourseAssociation? subjectCourseAssociation = await associations
            .OfType<GithubSubjectCourseAssociation>()
            .FirstOrDefaultAsync(gsc => gsc.GithubOrganizationName == organization, cancellationToken);

        if (subjectCourseAssociation is null)
            throw new EntityNotFoundException($"SubjectCourse with organization {organization} not found");

        return subjectCourseAssociation.SubjectCourse;
    }
}