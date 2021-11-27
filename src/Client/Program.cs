using FSH.BlazorWebAssembly.Client;
using FSH.BlazorWebAssembly.Client.Infrastructure.Extensions;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.AddClientServices();
builder.Build().RunAsync();