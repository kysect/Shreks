using ITMO.Dev.ASAP.Application.GithubWorkflow.Abstractions;
using ITMO.Dev.ASAP.Application.GithubWorkflow.BackgroundServices;
using ITMO.Dev.ASAP.Application.GithubWorkflow.OrganizationManagement;
using Microsoft.Extensions.DependencyInjection;

namespace ITMO.Dev.ASAP.Application.GithubWorkflow.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGithubWorkflowServices(this IServiceCollection services)
    {
        return services
            .AddScoped<ISubjectCourseGithubOrganizationManager, SubjectCourseGithubOrganizationManager>()
            .AddHostedService<GithubInvitingWorker>();
    }
}