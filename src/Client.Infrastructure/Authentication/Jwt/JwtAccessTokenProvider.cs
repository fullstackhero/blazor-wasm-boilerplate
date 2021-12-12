using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace FSH.BlazorWebAssembly.Client.Infrastructure.Authentication.Jwt;

// A simple implementation of IAccessTokenProvider right now just to be able to use the same
// interface for providing tokens to the signalr connection (in HubExtensions.TryInitialize)
internal class JwtAccessTokenProvider : IAccessTokenProvider
{
    private readonly ILocalStorageService _localStorage;

    public JwtAccessTokenProvider(ILocalStorageService localStorage) =>
        _localStorage = localStorage;

    public async ValueTask<AccessTokenResult> RequestAccessToken() =>
        new AccessTokenResult(
            AccessTokenResultStatus.Success,
            new AccessToken() { Value = await _localStorage.GetItemAsync<string>("authToken") },
            string.Empty);

    public ValueTask<AccessTokenResult> RequestAccessToken(AccessTokenRequestOptions options) =>
        throw new NotImplementedException();
}