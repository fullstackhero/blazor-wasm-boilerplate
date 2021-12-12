using FSH.BlazorWebAssembly.Client.Infrastructure.Authentication.AzureAd;
using FSH.BlazorWebAssembly.Client.Infrastructure.Authentication.Jwt;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace FSH.BlazorWebAssembly.Client.Infrastructure.Authentication;

internal static class Startup
{
    public static IServiceCollection AddAuthentication(this IServiceCollection services, IConfiguration config) =>
        config[nameof(AuthProvider)] switch
        {
            // AzureAd
            nameof(AuthProvider.AzureAd) => services
                .AddTransient<IAuthenticationService, AzureAdAuthenticationService>()
                .AddScoped<AzureAdAuthorizationMessageHandler>()
                .AddMsalAuthentication(options =>
                    {
                        config.Bind(nameof(AuthProvider.AzureAd), options.ProviderOptions.Authentication);
                        options.ProviderOptions.DefaultAccessTokenScopes.Add(
                            config[$"{nameof(AuthProvider.AzureAd)}:ApiScope"]);
                        options.ProviderOptions.LoginMode = "redirect";
                    })
                    .AddAccountClaimsPrincipalFactory<AzureAdClaimsPrincipalFactory>()
                    .Services,

            // Jwt
            _ => services
                .AddTransient<IAuthenticationService, JwtAuthenticationService>()
                .AddTransient<IAccessTokenProvider, JwtAccessTokenProvider>()
                .AddScoped<JwtAuthenticationStateProvider>()
                .AddScoped<AuthenticationStateProvider>(p => p.GetRequiredService<JwtAuthenticationStateProvider>())
                .AddScoped<JwtAuthenticationHeaderHandler>()
        };

    public static IHttpClientBuilder AddAuthenticationHandler(this IHttpClientBuilder builder, IConfiguration config) =>
        config[nameof(AuthProvider)] switch
        {
            // AzureAd
            nameof(AuthProvider.AzureAd) =>
                builder.AddHttpMessageHandler<AzureAd.AzureAdAuthorizationMessageHandler>(),

            // Jwt
            _ => builder.AddHttpMessageHandler<Jwt.JwtAuthenticationHeaderHandler>()
        };
}