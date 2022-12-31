using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Kysect.Shreks.WebUI.AdminPanel;
using Kysect.Shreks.WebUI.AdminPanel.Extensions;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var baseAddress = new Uri(builder.HostEnvironment.BaseAddress);
builder.Services.AddAdminPanel(baseAddress);

var app = builder.Build();

await app.RunAsync();