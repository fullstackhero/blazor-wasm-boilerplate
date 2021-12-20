using FSH.BlazorWebAssembly.Client.Infrastructure.ApiClient;
using FSH.BlazorWebAssembly.Shared.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace FSH.BlazorWebAssembly.Client.Infrastructure.Authentication.Jwt;

// A simple implementation of IAccessTokenProvider used for both the Api
// (JwtAuthenticationHeaderHandler) and SignalR (HubExtentions.TryInitialize)
internal class JwtAccessTokenProvider : IAccessTokenProvider
{
    // can't work with actual services in the constructor here, have to
    // use IServiceProvider, otherwise the app hangs at startup
    private readonly IServiceProvider _services;

    public JwtAccessTokenProvider(IServiceProvider serviceProvider) =>
        _services = serviceProvider;

    public async ValueTask<AccessTokenResult> RequestAccessToken()
    {
        var authStateProvider = _services.GetRequiredService<JwtAuthenticationStateProvider>();
        var authState = await authStateProvider.GetAuthenticationStateAsync();
        string? token = null;
        if (authState.User.Identity?.IsAuthenticated == true)
        {
            token = await authStateProvider.GetAuthTokenAsync();

            // Check if token needs to be refreshed (when its expiration time is less than 1 minute away)
            var expTime = authState.User.GetExpiration();
            var diff = expTime - DateTime.UtcNow;
            if (diff.TotalMinutes <= 1)
            {
                string? refreshToken = await authStateProvider.GetRefreshTokenAsync();
                var response = await _services.GetRequiredService<IAuthenticationService>()
                    .RefreshTokenAsync(new RefreshTokenRequest { Token = token, RefreshToken = refreshToken });
                if (response.Succeeded)
                {
                    token = response.Data?.Token;
                }
            }
        }

        return new AccessTokenResult(
            AccessTokenResultStatus.Success,
            new AccessToken() { Value = token },
            string.Empty);
    }

    public ValueTask<AccessTokenResult> RequestAccessToken(AccessTokenRequestOptions options) =>
        RequestAccessToken();
}