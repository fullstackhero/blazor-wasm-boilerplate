using FSH.BlazorWebAssembly.Client.Infrastructure.Authentication;
using FSH.BlazorWebAssembly.Client.Infrastructure.Managers;
using FSH.BlazorWebAssembly.Client.Infrastructure.Managers.Preferences;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MudBlazor;
using MudBlazor.Services;
using System.Globalization;
using System.Reflection;
using Toolbelt.Blazor.Extensions.DependencyInjection;

namespace FSH.BlazorWebAssembly.Client.Infrastructure.Extensions
{
    public static class WebAssemblyHostBuilderExtensions
    {
        private const string ClientName = "FullStackHero.API";
        public static WebAssemblyHostBuilder AddClientServices(this WebAssemblyHostBuilder builder, WebAssemblyHostConfiguration configs)
        {
            builder
                .Services
                .AddDistributedMemoryCache()
                .AddLocalization(options =>
                {
                    options.ResourcesPath = "Resources";
                })
                .AddAuthorizationCore(options =>
                {
                    RegisterPermissionClaims(options);
                })
                .AddBlazoredLocalStorage()
                .AddMudServices(configuration =>
                {
                    configuration.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomRight;
                    configuration.SnackbarConfiguration.HideTransitionDuration = 100;
                    configuration.SnackbarConfiguration.ShowTransitionDuration = 100;
                    configuration.SnackbarConfiguration.VisibleStateDuration = 3000;
                    configuration.SnackbarConfiguration.ShowCloseIcon = false;
                })
                .AddScoped<ClientPreferenceManager>()
                .AddScoped<ApplicationAuthenticationStateProvider>()
                .AddScoped<AuthenticationStateProvider, ApplicationAuthenticationStateProvider>()
                .AddTransient<AuthenticationHeaderHandler>()
                .AutoRegisterInterfaces<IManager>()
                .AutoRegisterInterfaces<IApiService>()
                .AddScoped(sp => sp
                    .GetRequiredService<IHttpClientFactory>()
                    .CreateClient(ClientName).EnableIntercept(sp))
                .AddHttpClient(ClientName, client =>
                {
                    client.DefaultRequestHeaders.AcceptLanguage.Clear();
                    client.DefaultRequestHeaders.AcceptLanguage.ParseAdd(CultureInfo.DefaultThreadCurrentCulture?.TwoLetterISOLanguageName);
                    client.BaseAddress = new Uri(configs["ServerOptions:BaseUri"]);
                })
                .AddHttpMessageHandler<AuthenticationHeaderHandler>();
            builder.Services.AddHttpClientInterceptor();
            return builder;
        }

        public static IServiceCollection AutoRegisterInterfaces<T>(this IServiceCollection services)
        {
            var @interface = typeof(T);

            var types = @interface
                .Assembly
                .GetExportedTypes()
                .Where(t => t.IsClass && !t.IsAbstract)
                .Select(t => new
                {
                    Service = t.GetInterface($"I{t.Name}"),
                    Implementation = t
                })
                .Where(t => t.Service != null);

            foreach (var type in types)
            {
                if (@interface.IsAssignableFrom(type.Service))
                {
                    services.AddTransient(type.Service, type.Implementation);
                }
            }

            return services;
        }

        private static void RegisterPermissionClaims(AuthorizationOptions options)
        {
            foreach (var prop in typeof(PermissionConstants).GetNestedTypes().SelectMany(c => c.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)))
            {
                object propertyValue = prop.GetValue(null);
                if (propertyValue is not null)
                {
                    options.AddPolicy(propertyValue.ToString(), policy => policy.RequireClaim(ClaimConstants.Permission, propertyValue.ToString()));
                }
            }
        }
    }
}