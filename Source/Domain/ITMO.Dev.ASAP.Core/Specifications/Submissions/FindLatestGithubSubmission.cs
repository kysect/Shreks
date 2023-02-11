using ITMO.Dev.ASAP.Core.SubmissionAssociations;
using ITMO.Dev.ASAP.Core.Submissions;

namespace ITMO.Dev.ASAP.Core.Specifications.Submissions;

public class FindLatestGithubSubmission : ISpecification<SubmissionAssociation, GithubSubmission>
{
    private readonly string _organization;
    private readonly long _prNumber;
    private readonly string _repository;

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