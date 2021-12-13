using FSH.BlazorWebAssembly.Shared.Identity;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace FSH.BlazorWebAssembly.Client.Infrastructure.Authentication.Jwt;

// A simple implementation of IAccessTokenProvider used by both JwtAuthenticationHeaderHandler and SignalR (HubExtentions.TryInitialize)
internal class JwtAccessTokenProvider : IAccessTokenProvider
{
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

            // Check if token needs to be refreshed
            string? exp = authState.User.FindFirstValue("exp");
            var expTime = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(exp));
            var diff = expTime - DateTime.UtcNow;
            if (diff.TotalMinutes <= 2)
            {
                string? refreshToken = await authStateProvider.GetRefreshTokenAsync();
                var response = await _services.GetRequiredService<IAuthenticationService>()
                    .RefreshTokenAsync(new RefreshTokenRequest(token, refreshToken));
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