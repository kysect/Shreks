using Kysect.Shreks.WebApi.Sdk.ControllerClients;
using Kysect.Shreks.WebApi.Sdk.ControllerClients.Implementations;
using Kysect.Shreks.WebApi.Sdk.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Kysect.Shreks.WebApi.Sdk.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddShreksSdk(this IServiceCollection collection, Uri baseAddress)
    {
        void AddClient<TClient, TImplementation>()
            where TClient : class
            where TImplementation : class, TClient
        {
            collection
                .AddHttpClient<TClient, TImplementation>(client => client.BaseAddress = baseAddress)
                .AddHttpMessageHandler(provider =>
                {
                    IIdentityProvider identityProvider = provider.GetRequiredService<IIdentityProvider>();
                    return new AuthorizationMessageHandlerDecorator(identityProvider);
                });
        }

        AddClient<IAssignmentClient, AssignmentClient>();
        AddClient<IGithubManagementClient, GithubManagementClient>();
        AddClient<IGoogleClient, GoogleClient>();
        AddClient<IGroupAssignmentClient, GroupAssignmentClient>();
        AddClient<IIdentityClient, IdentityClient>();
        AddClient<IStudentClient, StudentClient>();
        AddClient<IStudyGroupClient, StudyGroupClient>();
        AddClient<ISubjectClient, SubjectClient>();
        AddClient<ISubjectCourseClient, SubjectCourseClient>();
        AddClient<ISubjectCourseGroupClient, SubjectCourseGroupClient>();

        return collection;
    }
}