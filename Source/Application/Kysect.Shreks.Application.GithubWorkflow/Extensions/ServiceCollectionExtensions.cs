using Kysect.Shreks.Application.GithubWorkflow.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Kysect.Shreks.Application.GithubWorkflow.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGithubWorkflowServices(this IServiceCollection services)
    {
        return services
            .AddScoped<ISubjectCourseGithubOrganizationManager, SubjectCourseGithubOrganizationManager>()
            .AddScoped<GithubSubmissionFactory>()
            .AddScoped<IShreksWebhookEventProcessor, ShreksWebhookEventProcessor>()
            .AddHostedService<GithubInvitingWorker>();
    }
}
