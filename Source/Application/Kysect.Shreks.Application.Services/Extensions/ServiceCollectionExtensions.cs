using Kysect.Shreks.Application.Abstractions.SubjectCourses;
using Kysect.Shreks.Application.Abstractions.Submissions;
using Microsoft.Extensions.DependencyInjection;

namespace Kysect.Shreks.Application.Services.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<ISubjectCourseService, SubjectCourseService>();
        services.AddScoped<ISubmissionWorkflowService, SubmissionWorkflowService>();

        return services;
    }
}