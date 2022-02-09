using FSH.BlazorWebAssembly.Client.Infrastructure.Auth.AzureAd;
using FSH.BlazorWebAssembly.Client.Infrastructure.Auth.Jwt;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace FSH.BlazorWebAssembly.Client.Infrastructure.Auth;

internal static class Startup
{
    public static IServiceCollection AddAuthentication(this IServiceCollection services, IConfiguration config) =>
        config[nameof(AuthProvider)] switch
        {
            // AzureAd
            nameof(AuthProvider.AzureAd) => services
                .AddScoped<IAuthenticationService, AzureAdAuthenticationService>()
                .AddScoped<AzureAdAuthorizationMessageHandler>()
                .AddMsalAuthentication(options =>
                    {
                        config.Bind(nameof(AuthProvider.AzureAd), options.ProviderOptions.Authentication);
                        options.ProviderOptions.DefaultAccessTokenScopes.Add(
                            config[$"{nameof(AuthProvider.AzureAd)}:{ConfigNames.ApiScope}"]);
                        options.ProviderOptions.LoginMode = "redirect";
                    })
                    .AddAccountClaimsPrincipalFactory<AzureAdClaimsPrincipalFactory>()
                    .Services,

            // Jwt
            _ => services
                .AddScoped<AuthenticationStateProvider, JwtAuthenticationService>()
                .AddScoped(sp => (IAuthenticationService)sp.GetRequiredService<AuthenticationStateProvider>())
                .AddScoped(sp => (IAccessTokenProvider)sp.GetRequiredService<AuthenticationStateProvider>())
                .AddScoped<IAccessTokenProviderAccessor, AccessTokenProviderAccessor>()
                .AddScoped<JwtAuthenticationHeaderHandler>()
        };

    public static IHttpClientBuilder AddAuthenticationHandler(this IHttpClientBuilder builder, IConfiguration config) =>
        config[nameof(AuthProvider)] switch
        {
            // AzureAd
            nameof(AuthProvider.AzureAd) =>
                builder.AddHttpMessageHandler<AzureAdAuthorizationMessageHandler>(),

            // Jwt
            _ => builder.AddHttpMessageHandler<JwtAuthenticationHeaderHandler>()
        };
}