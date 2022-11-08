using Blazored.LocalStorage;
using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;
using Kysect.Shreks.WebApi.Sdk;
using Kysect.Shreks.WebUI.AdminPanel.ExceptionHandling;
using Kysect.Shreks.WebUI.AdminPanel.Identity;
using Kysect.Shreks.WebUI.AdminPanel.Tools;
using Microsoft.AspNetCore.Components.Authorization;

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

        var exceptionDisplayConfiguration = new ExceptionDisplayConfiguration(TimeSpan.FromSeconds(6));
        collection.AddSingleton(exceptionDisplayConfiguration);

        collection.AddScoped<IIdentityManager, LocalStorageIdentityManager>();
        collection.AddScoped<IIdentityService, IdentityService>();

        collection.AddSingleton<ExceptionManager>();
        collection.AddSingleton<IExceptionSink>(x => x.GetRequiredService<ExceptionManager>());
        collection.AddSingleton<IExceptionStore>(x => x.GetRequiredService<ExceptionManager>());
        collection.AddSingleton<ISafeExecutor, SafeExecutor>();

        collection.AddOptions();
        collection.AddAuthorizationCore();
        collection.AddScoped<IdentityStateProvider>();
        collection.AddScoped<AuthenticationStateProvider>(x => x.GetRequiredService<IdentityStateProvider>());

        collection.AddHttpClient(Constants.ClientName).AddHttpMessageHandler(p =>
            new AuthorizationMessageHandlerDecorator(p.GetRequiredService<IIdentityManager>()));

        collection.AddClients(baseUrl);
    }

    private static void AddClients(this IServiceCollection collection, string baseUrl)
    {
        collection.AddClient(x => new IdentityClient(baseUrl, x));
        collection.AddClient(x => new SubjectClient(baseUrl, x));
        collection.AddClient(x => new SubjectCourseClient(baseUrl, x));
        collection.AddClient(x => new GoogleClient(baseUrl, x));
        collection.AddClient(x => new GithubManagementClient(baseUrl, x));
        collection.AddClient(x => new StudentClient(baseUrl, x));
        collection.AddClient(x => new GroupAssignmentClient(baseUrl, x));
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