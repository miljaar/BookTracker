using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using BookTracker.Blazor;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? throw new InvalidOperationException("ApiBaseUrl is missing from appsettings.");

builder.Services.AddScoped(_ =>
    new HttpClient
    {
        BaseAddress = new Uri(apiBaseUrl)
    });

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

await builder.Build().RunAsync();
