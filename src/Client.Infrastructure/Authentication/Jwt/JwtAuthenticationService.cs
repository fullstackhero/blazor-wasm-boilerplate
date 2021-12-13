using FSH.BlazorWebAssembly.Client.Infrastructure.Extensions;
using FSH.BlazorWebAssembly.Shared.Identity;
using Microsoft.AspNetCore.Components;

namespace FSH.BlazorWebAssembly.Client.Infrastructure.Authentication.Jwt;

public class JwtAuthenticationService : IAuthenticationService
{
    private readonly HttpClient _httpClient;
    private readonly JwtAuthenticationStateProvider _authStateProvider;
    private readonly NavigationManager _navigationManager;

    public JwtAuthenticationService(
        HttpClient httpClient,
        JwtAuthenticationStateProvider authStateProvider,
        NavigationManager navigationManager)
    {
        _httpClient = httpClient;
        _authStateProvider = authStateProvider;
        _navigationManager = navigationManager;
    }

    public AuthProvider ProviderType => AuthProvider.Jwt;

    public async Task<IResult> LoginAsync(TokenRequest model)
    {
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add(HeaderConstants.Tenant, model.Tenant);
        var response = await _httpClient.PostAsJsonAsync(TokenEndpoints.AuthenticationEndpoint, model);
        var result = await response.ToResultAsync<TokenResponse>();
        if (result.Succeeded)
        {
            string? token = result.Data?.Token;
            string? refreshToken = result.Data?.RefreshToken;

            if (string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(refreshToken))
            {
                return Result.Fail("Invalid token received.");
            }

            await _authStateProvider.MarkUserAsLoggedInAsync(token, refreshToken);

            return await Result.SuccessAsync();
        }
        else
        {
            return await Result.FailAsync(result.Exception);
        }
    }

    public async Task<IResult> LogoutAsync()
    {
        await _authStateProvider.MarkUserAsLoggedOutAsync();

        _navigationManager.NavigateTo("/login");
        return await Result.SuccessAsync();
    }

    public async Task<IResult<TokenResponse>> RefreshTokenAsync(RefreshTokenRequest request)
    {
        var authState = await _authStateProvider.GetAuthenticationStateAsync();
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add(HeaderConstants.Tenant, authState.User.GetTenant());
        var response = await _httpClient.PostAsJsonAsync(TokenEndpoints.Refresh, request);
        var tokenResponse = await response.ToResultAsync<TokenResponse>();
        if (tokenResponse.Succeeded && tokenResponse.Data is not null)
        {
            await _authStateProvider.SaveAuthTokens(tokenResponse.Data.Token, tokenResponse.Data.RefreshToken);
        }

        return tokenResponse;
    }
}