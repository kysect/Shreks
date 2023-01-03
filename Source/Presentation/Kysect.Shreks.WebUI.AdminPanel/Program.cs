using Kysect.Shreks.WebUI.AdminPanel;
using Kysect.Shreks.WebUI.AdminPanel.Extensions;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var baseAddress = new Uri(builder.HostEnvironment.BaseAddress);
builder.Services.AddAdminPanel(baseAddress);

WebAssemblyHost app = builder.Build();

await app.RunAsync();