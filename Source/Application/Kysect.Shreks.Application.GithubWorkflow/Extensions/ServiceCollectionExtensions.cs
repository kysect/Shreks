using Kysect.Shreks.Application.GithubWorkflow.Abstractions;
using Kysect.Shreks.Application.GithubWorkflow.BackgroundServices;
using Kysect.Shreks.Application.GithubWorkflow.OrganizationManagement;
using Microsoft.Extensions.DependencyInjection;

namespace Kysect.Shreks.Application.GithubWorkflow.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGithubWorkflowServices(this IServiceCollection services)
    {
        return services
            .AddScoped<ISubjectCourseGithubOrganizationManager, SubjectCourseGithubOrganizationManager>()
            .AddHostedService<GithubInvitingWorker>();
    }
}
