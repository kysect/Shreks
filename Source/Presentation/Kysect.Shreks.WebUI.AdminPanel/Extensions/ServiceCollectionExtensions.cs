using Blazored.LocalStorage;
using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;
using Kysect.Shreks.WebApi.Sdk.Extensions;
using Kysect.Shreks.WebApi.Sdk.Identity;
using Kysect.Shreks.WebUI.AdminPanel.ExceptionHandling;
using Kysect.Shreks.WebUI.AdminPanel.Identity;
using Kysect.Shreks.WebUI.AdminPanel.Tools;
using Microsoft.AspNetCore.Components.Authorization;

namespace Kysect.Shreks.WebUI.AdminPanel.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddAdminPanel(this IServiceCollection collection, Uri baseUrl)
    {
        collection.AddBlazoredLocalStorage();
        collection.AddBlazorise();

        collection
            .AddBlazorise(options => options.Immediate = true)
            .AddBootstrapProviders()
            .AddFontAwesomeIcons();

        var exceptionDisplayConfiguration = new ExceptionDisplayConfiguration(TimeSpan.FromSeconds(6));
        collection.AddSingleton(exceptionDisplayConfiguration);

        collection.AddScoped<LocalStorageIdentityManager>();
        collection.AddScoped<IIdentityManager>(x => x.GetRequiredService<LocalStorageIdentityManager>());
        collection.AddScoped<IIdentityProvider>(x => x.GetRequiredService<LocalStorageIdentityManager>());

        collection.AddScoped<IIdentityService, IdentityService>();

        collection.AddScoped<ExceptionManager>();
        collection.AddScoped<IExceptionSink>(x => x.GetRequiredService<ExceptionManager>());
        collection.AddScoped<IExceptionStore>(x => x.GetRequiredService<ExceptionManager>());
        collection.AddScoped<ISafeExecutor, SafeExecutor>();

        collection.AddOptions();
        collection.AddAuthorizationCore();
        collection.AddScoped<IdentityStateProvider>();
        collection.AddScoped<AuthenticationStateProvider>(x => x.GetRequiredService<IdentityStateProvider>());

        collection.AddShreksSdk(baseUrl);
    }
}