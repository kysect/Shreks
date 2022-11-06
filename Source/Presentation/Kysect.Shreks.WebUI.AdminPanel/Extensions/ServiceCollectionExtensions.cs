using Blazored.LocalStorage;
using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;
using Kysect.Shreks.WebApi.Sdk;
using Kysect.Shreks.WebUI.AdminPanel.Identity;
using Kysect.Shreks.WebUI.AdminPanel.Tools;

namespace Kysect.Shreks.WebUI.AdminPanel.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddAdminPanel(this IServiceCollection collection, string baseUrl)
    {
        collection.AddBlazoredLocalStorage();
        collection.AddBlazorise();

        collection
            .AddBlazorise(options => options.Immediate = true)
            .AddBootstrapProviders()
            .AddFontAwesomeIcons();

        collection.AddHttpClient(Constants.ClientName).AddHttpMessageHandler(p =>
            new AuthorizationMessageHandlerDecorator(p.GetRequiredService<IIdentityManager>()));
        
        collection.AddClient(x => new IdentityClient(baseUrl, x));
        collection.AddClient(x => new SubjectClient(baseUrl, x));
        collection.AddClient(x => new SubjectCourseClient(baseUrl, x));
        collection.AddClient(x => new GoogleClient(baseUrl, x));
        collection.AddClient(x => new GithubManagementClient(baseUrl, x));
        collection.AddClient(x => new StudentClient(baseUrl, x));
        collection.AddClient(x => new GroupAssignmentClient(baseUrl, x));

        collection.AddScoped<IIdentityManager, IdentityManager>();
        collection.AddScoped<IIdentityService, IdentityService>();
    }

    private static void AddClient<T>(this IServiceCollection collection, Func<HttpClient, T> clientFactory)
        where T : class
    {
        collection.AddScoped(p =>
        {
            var factory = p.GetRequiredService<IHttpClientFactory>();
            var client = factory.CreateClient(Constants.ClientName);

            return clientFactory.Invoke(client);
        });
    }
}