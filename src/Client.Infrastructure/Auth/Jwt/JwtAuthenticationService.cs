﻿using FSH.BlazorWebAssembly.Client.Infrastructure.ApiClient;
using FSH.WebApi.Shared.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace FSH.BlazorWebAssembly.Client.Infrastructure.Auth.Jwt;

public class JwtAuthenticationService : AuthenticationStateProvider, IAuthenticationService, IAccessTokenProvider
{
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    private readonly ILocalStorageService _localStorage;
    private readonly ITokensClient _tokensClient;
    private readonly IPersonalClient _personalClient;
    private readonly NavigationManager _navigation;

    public AuthProvider ProviderType => AuthProvider.Jwt;

    public JwtAuthenticationService(ILocalStorageService localStorage, IPersonalClient personalClient, ITokensClient tokensClient, NavigationManager navigation)
    {
        _localStorage = localStorage;
        _personalClient = personalClient;
        _tokensClient = tokensClient;
        _navigation = navigation;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        string cachedToken = await GetCachedAuthTokenAsync();
        if (string.IsNullOrWhiteSpace(cachedToken))
        {
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        // Generate claimsIdentity from cached token
        var claimsIdentity = new ClaimsIdentity(GetClaimsFromJwt(cachedToken), "jwt");

        // Add cached permissions as claims
        if (await GetCachedPermissionsAsync() is List<string> cachedPermissions)
        {
            claimsIdentity.AddClaims(cachedPermissions.Select(p => new Claim(FSHClaims.Permission, p)));
        }

        return new AuthenticationState(new ClaimsPrincipal(claimsIdentity));
    }

    public void NavigateToExternalLogin(string returnUrl) =>
        throw new NotImplementedException();

    public async Task<bool> LoginAsync(string tenantId, TokenRequest request)
    {
        var tokenResponse = await _tokensClient.GetTokenAsync(tenantId, request);

        string? token = tokenResponse.Token;
        string? refreshToken = tokenResponse.RefreshToken;

        if (string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(refreshToken))
        {
            return false;
        }

        await CacheAuthTokens(token, refreshToken);

        // Get permissions for the current user and add them to the cache
        var permissions = await _personalClient.GetPermissionsAsync();
        await CachePermissions(permissions);

        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());

        return true;
    }

    public async Task LogoutAsync()
    {
        await ClearCacheAsync();

        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());

        _navigation.NavigateTo("/login");
    }

    public async Task ReLoginAsync(string returnUrl)
    {
        await LogoutAsync();
        _navigation.NavigateTo(returnUrl);
    }

    public async ValueTask<AccessTokenResult> RequestAccessToken()
    {
        var authState = await GetAuthenticationStateAsync();
        if (authState.User.Identity?.IsAuthenticated is not true)
        {
            return new AccessTokenResult(AccessTokenResultStatus.RequiresRedirect, null, "/login");
        }

        // We make sure the access token is only refreshed by one thread at a time. The other ones have to wait.
        await _semaphore.WaitAsync();
        try
        {
            string? token = await GetCachedAuthTokenAsync();

            // Check if token needs to be refreshed (when its expiration time is less than 1 minute away)
            var expTime = authState.User.GetExpiration();
            var diff = expTime - DateTime.UtcNow;
            if (diff.TotalMinutes <= 1)
            {
                string? refreshToken = await GetCachedRefreshTokenAsync();
                (bool succeeded, var response) = await TryRefreshTokenAsync(new RefreshTokenRequest { Token = token, RefreshToken = refreshToken });
                if (!succeeded)
                {
                    return new AccessTokenResult(AccessTokenResultStatus.RequiresRedirect, null, "/login");
                }

                token = response?.Token;
            }

            return new AccessTokenResult(AccessTokenResultStatus.Success, new AccessToken() { Value = token }, string.Empty);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public ValueTask<AccessTokenResult> RequestAccessToken(AccessTokenRequestOptions options) =>
        RequestAccessToken();

    private async Task<(bool Succeeded, TokenResponse? Token)> TryRefreshTokenAsync(RefreshTokenRequest request)
    {
        var authState = await GetAuthenticationStateAsync();
        string? tenantKey = authState.User.GetTenant();
        if (string.IsNullOrWhiteSpace(tenantKey))
        {
            throw new InvalidOperationException("Can't refresh token when user is not logged in!");
        }

        try
        {
            var tokenResponse = await _tokensClient.RefreshAsync(tenantKey, request);

            await CacheAuthTokens(tokenResponse.Token, tokenResponse.RefreshToken);

            return (true, tokenResponse);
        }
        catch (ApiException<ErrorResult>)
        {
            return (false, null);
        }
    }

    private async ValueTask CacheAuthTokens(string? token, string? refreshToken)
    {
        await _localStorage.SetItemAsync(StorageConstants.Local.AuthToken, token);
        await _localStorage.SetItemAsync(StorageConstants.Local.RefreshToken, refreshToken);
    }

    private ValueTask CachePermissions(ICollection<string> permissions) =>
        _localStorage.SetItemAsync(StorageConstants.Local.Permissions, permissions);

    private async Task ClearCacheAsync()
    {
        await _localStorage.RemoveItemAsync(StorageConstants.Local.AuthToken);
        await _localStorage.RemoveItemAsync(StorageConstants.Local.RefreshToken);
        await _localStorage.RemoveItemAsync(StorageConstants.Local.Permissions);
    }

    private ValueTask<string> GetCachedAuthTokenAsync() =>
        _localStorage.GetItemAsync<string>(StorageConstants.Local.AuthToken);

    private ValueTask<string> GetCachedRefreshTokenAsync() =>
        _localStorage.GetItemAsync<string>(StorageConstants.Local.RefreshToken);

    private ValueTask<ICollection<string>> GetCachedPermissionsAsync() =>
        _localStorage.GetItemAsync<ICollection<string>>(StorageConstants.Local.Permissions);

    private IEnumerable<Claim> GetClaimsFromJwt(string jwt)
    {
        var claims = new List<Claim>();
        string payload = jwt.Split('.')[1];
        byte[] jsonBytes = ParseBase64WithoutPadding(payload);
        var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

        if (keyValuePairs is not null)
        {
            keyValuePairs.TryGetValue(ClaimTypes.Role, out object? roles);

            if (roles is not null)
            {
                string? rolesString = roles.ToString();
                if (!string.IsNullOrEmpty(rolesString))
                {
                    if (rolesString.Trim().StartsWith("["))
                    {
                        string[]? parsedRoles = JsonSerializer.Deserialize<string[]>(rolesString);

                        if (parsedRoles is not null)
                        {
                            claims.AddRange(parsedRoles.Select(role => new Claim(ClaimTypes.Role, role)));
                        }
                    }
                    else
                    {
                        claims.Add(new Claim(ClaimTypes.Role, rolesString));
                    }
                }

                keyValuePairs.Remove(ClaimTypes.Role);
            }

            claims.AddRange(keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString() ?? string.Empty)));
        }

        return claims;
    }

    private byte[] ParseBase64WithoutPadding(string payload)
    {
        payload = payload.Trim().Replace('-', '+').Replace('_', '/');
        string base64 = payload.PadRight(payload.Length + ((4 - (payload.Length % 4)) % 4), '=');
        return Convert.FromBase64String(base64);
    }
}