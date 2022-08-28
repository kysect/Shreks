using Kysect.Shreks.Core.SubmissionAssociations;
using Kysect.Shreks.Core.Submissions;

namespace Kysect.Shreks.Core.Specifications.Submissions;

public class FindLatestGithubSubmission : ISpecification<SubmissionAssociation, GithubSubmission>
{
    private readonly string _organization;
    private readonly string _repository;
    private readonly long _prNumber;

    public FindLatestGithubSubmission(string organization, string repository, long prNumber)
    {
        _organization = organization;
        _repository = repository;
        _prNumber = prNumber;
    }

    public IQueryable<GithubSubmission> Apply(IQueryable<SubmissionAssociation> query)
    {
        return query
            .OfType<GithubSubmissionAssociation>()
            .Where(a =>
                a.Organization == _organization
                && a.Repository == _repository
                && a.PrNumber == _prNumber)
            .OrderByDescending(a => a.Submission.SubmissionDate)
            .Select(a => a.Submission)
            .OfType<GithubSubmission>();
    }
}