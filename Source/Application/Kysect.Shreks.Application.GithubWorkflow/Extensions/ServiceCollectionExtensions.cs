using Kysect.Shreks.Application.Commands.Processors;
using Kysect.Shreks.Application.GithubWorkflow.Abstractions;
using Kysect.Shreks.Application.GithubWorkflow.BackgroundServices;
using Kysect.Shreks.Application.GithubWorkflow.OrganizationManagement;
using Kysect.Shreks.Application.GithubWorkflow.Submissions;
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
            .AddScoped<ISubmissionService, SubmissionService>()
            .AddHostedService<GithubInvitingWorker>();
    }
}
