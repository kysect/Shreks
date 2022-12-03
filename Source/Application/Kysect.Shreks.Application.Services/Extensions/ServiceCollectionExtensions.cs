using Kysect.Shreks.Application.Abstractions.SubjectCourses;
using Microsoft.Extensions.DependencyInjection;

namespace Kysect.Shreks.Application.Services.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<ISubjectCourseService, SubjectCourseService>();

        return services;
    }
}