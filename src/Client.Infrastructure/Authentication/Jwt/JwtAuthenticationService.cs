using FSH.BlazorWebAssembly.Client.Infrastructure.ApiClient;
using FSH.BlazorWebAssembly.Client.Infrastructure.Extensions;
using Microsoft.AspNetCore.Components;
using Result = FSH.BlazorWebAssembly.Shared.Wrapper.Result;

namespace FSH.BlazorWebAssembly.Client.Infrastructure.Authentication.Jwt;

public class JwtAuthenticationService : IAuthenticationService
{
    private readonly ITokensClient _tokensClient;
    private readonly JwtAuthenticationStateProvider _authStateProvider;
    private readonly NavigationManager _navigationManager;

    public JwtAuthenticationService(
        ITokensClient tokensClient,
        JwtAuthenticationStateProvider authStateProvider,
        NavigationManager navigationManager)
    {
        _tokensClient = tokensClient;
        _authStateProvider = authStateProvider;
        _navigationManager = navigationManager;
    }

    public AuthProvider ProviderType => AuthProvider.Jwt;

    public async Task<IResult> LoginAsync(string tenantKey, TokenRequest request)
    {
        var result = await _tokensClient.GetTokenAsync(tenantKey, request);
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
            return await Result.FailAsync(result.Messages?.FirstOrDefault());
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
        string? tenantKey = authState.User.GetTenant();
        if (string.IsNullOrWhiteSpace(tenantKey))
        {
            throw new InvalidOperationException("Can't refresh token when user is not logged in!");
        }

        var tokenResponse = await _tokensClient.RefreshAsync(tenantKey, request);
        if (tokenResponse.Succeeded && tokenResponse.Data is not null)
        {
            await _authStateProvider.SaveAuthTokens(tokenResponse.Data.Token, tokenResponse.Data.RefreshToken);
            return await Result<TokenResponse>.SuccessAsync(tokenResponse.Data);
        }

        return await Result<TokenResponse>.FailAsync(tokenResponse.Messages?.ToList() ?? new ());
    }
}