using ITMO.Dev.ASAP.Common.Exceptions;
using ITMO.Dev.ASAP.Core.Study;
using ITMO.Dev.ASAP.Core.SubjectCourseAssociations;
using Microsoft.EntityFrameworkCore;

namespace ITMO.Dev.ASAP.Application.GithubWorkflow.Extensions;

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