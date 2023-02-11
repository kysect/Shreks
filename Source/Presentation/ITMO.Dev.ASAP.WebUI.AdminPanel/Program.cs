using ITMO.Dev.ASAP.WebUI.AdminPanel;
using ITMO.Dev.ASAP.WebUI.AdminPanel.Extensions;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddAdminPanel(builder.HostEnvironment);

WebAssemblyHost app = builder.Build();

await app.RunAsync();