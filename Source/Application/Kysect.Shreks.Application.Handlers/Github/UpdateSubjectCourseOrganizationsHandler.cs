using Kysect.Shreks.Core.Extensions;
using Kysect.Shreks.Core.Specifications.Github;
using Kysect.Shreks.Core.SubjectCourseAssociations;
using Kysect.Shreks.DataAccess.Abstractions;
using Kysect.Shreks.Integration.Github.Invites;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using static Kysect.Shreks.Application.Abstractions.Github.Commands.UpdateSubjectCourseOrganizations;

namespace Kysect.Shreks.Application.Handlers.Github;

public class UpdateSubjectCourseOrganizationsHandler : IRequestHandler<Command, Response>
{
    private readonly ILogger<UpdateSubjectCourseOrganizationsHandler> _logger;
    private readonly ISubjectCourseGithubOrganizationInviteSender _inviteSender;
    private readonly ISubjectCourseGithubOrganizationRepositoryManager _repositoryManager;
    private readonly IShreksDatabaseContext _context;

    public UpdateSubjectCourseOrganizationsHandler(
        ILogger<UpdateSubjectCourseOrganizationsHandler> logger,
        ISubjectCourseGithubOrganizationInviteSender inviteSender,
        ISubjectCourseGithubOrganizationRepositoryManager repositoryManager, IShreksDatabaseContext context)
    {
        _logger = logger;
        _inviteSender = inviteSender;
        _repositoryManager = repositoryManager;
        _context = context;
    }

    public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
    {
        List<GithubSubjectCourseAssociation> githubSubjectCourseAssociations = await _context
            .SubjectCourseAssociations
            .OfType<GithubSubjectCourseAssociation>()
            .ToListAsync(cancellationToken);

        foreach (GithubSubjectCourseAssociation subjectAssociation in githubSubjectCourseAssociations)
        {
            List<string> usernames = await _context
                .SubjectCourseGroups
                .WithSpecification(new GetSubjectCourseGithubUsers(subjectAssociation.SubjectCourse.Id))
                .Select(association => association.GithubUsername)
                .ToListAsync(cancellationToken: cancellationToken);

            await _inviteSender.Invite(subjectAssociation.GithubOrganizationName, usernames);
            await GenerateRepositories(_repositoryManager, usernames, subjectAssociation.GithubOrganizationName, subjectAssociation.TemplateRepositoryName);
        }

        return new Response();
    }

    private async Task GenerateRepositories(
        ISubjectCourseGithubOrganizationRepositoryManager repositoryManager,
        IReadOnlyCollection<string> usernames,
        string organizationName,
        string templateName)
    {
        IReadOnlyCollection<string> repositories = await repositoryManager.GetRepositories(organizationName);

        if (!repositories.Contains(templateName))
        {
            string message = $"No template repository found for organization {organizationName}";
            _logger.LogWarning(message);
            return;
        }

        foreach (string username in usernames)
        {
            string newRepositoryName = username;

            if (repositories.Any(r => r.Equals(newRepositoryName, StringComparison.OrdinalIgnoreCase)))
                continue;

            try
            {
                await repositoryManager.CreateRepositoryFromTemplate(organizationName, newRepositoryName, templateName);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to create repo for {username}", username);
            }

            try
            {
                await repositoryManager.AddAdminPermission(organizationName, newRepositoryName, username);
            }
            catch (Exception e) when (e.Message == "Invalid Status Code returned. Expected a 204 or a 404")
            {
                _logger.LogWarning(e, $"Octokit return wrong error code for {username}.");
                continue;
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Failed to add user {username} as repo admin.");
                continue;
            }

            _logger.LogInformation("Successfully created repository for user {User}", username);
        }
    }
}