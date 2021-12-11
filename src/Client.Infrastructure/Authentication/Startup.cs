using Microsoft.Extensions.DependencyInjection;

namespace FSH.BlazorWebAssembly.Client.Infrastructure.Authentication;

internal static class Startup
{
    public static IServiceCollection AddAuthentication(this IServiceCollection services, IConfiguration config) =>
        config[nameof(AuthProvider)] switch
        {
            // AzureAd
            nameof(AuthProvider.AzureAd) => services
                .AddTransient<IAuthenticationService, AzureAd.AzureAdAuthenticationService>()
                .AddScoped<AzureAd.ApiAuthorizationMessageHandler>()
                .AddMsalAuthentication(options =>
                    {
                        config.Bind(nameof(AuthProvider.AzureAd), options.ProviderOptions.Authentication);
                        options.ProviderOptions.DefaultAccessTokenScopes.Add(config[$"{nameof(AuthProvider.AzureAd)}:ApiScope"]);
                        options.ProviderOptions.LoginMode = "redirect";
                    })
                    .Services,

            // Jwt
            _ => services
                .AddTransient<IAuthenticationService, Jwt.JwtAuthenticationService>()
                .AddScoped<AuthenticationStateProvider, Jwt.ApplicationAuthenticationStateProvider>()
                .AddScoped<AuthenticationStateProvider>(p => p.GetRequiredService<Jwt.ApplicationAuthenticationStateProvider>())
                .AddTransient<Jwt.AuthenticationHeaderHandler>()
        };

    public static IHttpClientBuilder AddAuthenticationHandler(this IHttpClientBuilder builder, IConfiguration config) =>
        config[nameof(AuthProvider)] switch
        {
            // AzureAd
            nameof(AuthProvider.AzureAd) =>
                builder.AddHttpMessageHandler<AzureAd.ApiAuthorizationMessageHandler>(),

            // Jwt
            _ => builder.AddHttpMessageHandler<Jwt.AuthenticationHeaderHandler>()
        };
}