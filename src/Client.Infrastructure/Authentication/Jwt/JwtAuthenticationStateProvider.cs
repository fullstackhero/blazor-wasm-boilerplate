using FSH.BlazorWebAssembly.Client.Infrastructure.ApiClient;

namespace FSH.BlazorWebAssembly.Client.Infrastructure.Authentication.Jwt;

public class JwtAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly ILocalStorageService _localStorage;
    private readonly IUsersClient _usersClient;

    public JwtAuthenticationStateProvider(ILocalStorageService localStorage, IUsersClient usersClient) =>
        (_localStorage, _usersClient) = (localStorage, usersClient);

    public async Task MarkUserAsLoggedInAsync(string token, string refreshToken)
    {
        await SaveAuthTokens(token, refreshToken);

        // get userid from token
        var claims = GetClaimsFromJwt(token);
        string? userId = claims.SingleOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        if (!string.IsNullOrWhiteSpace(userId))
        {
            // get permissions for this user
            var permissionResult = await _usersClient.GetPermissionsAsync(userId);
            if (permissionResult.Succeeded && permissionResult.Data is not null)
            {
                // store them in localstorage
                await _localStorage.SetItemAsync(
                    StorageConstants.Local.Permissions,
                    permissionResult.Data
                        .Where(p => !string.IsNullOrWhiteSpace(p.Permission))
                        .Select(p => p.Permission!)
                        .ToList());
            }
        }

        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public async Task MarkUserAsLoggedOutAsync()
    {
        await _localStorage.RemoveItemAsync(StorageConstants.Local.AuthToken);
        await _localStorage.RemoveItemAsync(StorageConstants.Local.RefreshToken);
        await _localStorage.RemoveItemAsync(StorageConstants.Local.ImageUri);
        await _localStorage.RemoveItemAsync(StorageConstants.Local.Permissions);

        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public async ValueTask SaveAuthTokens(string? token, string? refreshToken)
    {
        await _localStorage.SetItemAsync(StorageConstants.Local.AuthToken, token);
        await _localStorage.SetItemAsync(StorageConstants.Local.RefreshToken, refreshToken);
    }

    public ValueTask<string> GetAuthTokenAsync() =>
        _localStorage.GetItemAsync<string>(StorageConstants.Local.AuthToken);

    public ValueTask<string> GetRefreshTokenAsync() =>
        _localStorage.GetItemAsync<string>(StorageConstants.Local.RefreshToken);

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        string savedToken = await GetAuthTokenAsync();
        if (string.IsNullOrWhiteSpace(savedToken))
        {
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        // Generate claimsIdentity from saved token
        var claimsIdentity = new ClaimsIdentity(GetClaimsFromJwt(savedToken), "jwt");

        // Add permission claims from local storage
        if (await _localStorage.GetItemAsync<List<string>>(StorageConstants.Local.Permissions) is List<string> permissionClaims)
        {
            claimsIdentity.AddClaims(permissionClaims.Select(p => new Claim(ClaimConstants.Permission, p)));
        }

        return new AuthenticationState(new ClaimsPrincipal(claimsIdentity));
    }

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